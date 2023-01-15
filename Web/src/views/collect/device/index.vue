<template>
	<div class="sys-device-container">
		<el-card shadow="hover" :body-style="{ paddingBottom: '0' }">
			<el-form :model="queryParams" ref="queryForm" :inline="true">
				<el-form-item label="设备名称" prop="name">
					<el-input placeholder="设备名称" clearable @keyup.enter="handleQuery" v-model="queryParams.name" />
				</el-form-item>
				<el-form-item label="驱动名称" prop="driverAssembleName">
					<el-input placeholder="驱动名称" clearable @keyup.enter="handleQuery"
						v-model="queryParams.driverAssembleName" />
				</el-form-item>

				<el-form-item>
					<el-button icon="ele-Refresh" @click="resetQuery"> 重置 </el-button>
					<el-button type="primary" icon="ele-Search" @click="handleQuery" v-auth="'device:page'"> 查询
					</el-button>
					<el-button icon="ele-Plus" @click="openAddDevice" v-auth="'device:add'"> 新增 </el-button>
					<el-button icon="ele-FolderOpened" @click="exportDevice" v-auth="'device:export'"> 导出 </el-button>
					<el-button icon="ele-UploadFilled" @click="dialogVisible = true" v-auth="'device:import'"> 导入
					</el-button>
					<el-button icon="ele-Refresh" @click="reStartDevice(0)" v-auth="'device:update'"> 全部重启
					</el-button>
				</el-form-item>
			</el-form>
		</el-card>

		<el-card shadow="hover" style="margin-top: 8px">
			<el-table :data="deviceData" style="width: 100%" v-loading="loading" border>
				<el-table-column type="index" label="序号" width="55" align="center" fixed />
				<el-table-column prop="name" label="设备名称" width="120" fixed show-overflow-tooltip />
				<el-table-column prop="description" label="描述" show-overflow-tooltip />
				<el-table-column prop="driverAssembleName" label="驱动名称" width="150" show-overflow-tooltip />

				<el-table-column label="启用" width="70" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-switch v-model="scope.row.invokeEnable" :active-value="true" :inactive-value="false"
							size="small" @change="changeStatus(scope.row)" v-auth="'device:setStatus'" />
					</template>
				</el-table-column>


				<el-table-column prop="invokeInterval" label="读取间隔" width="120" show-overflow-tooltip />

				<el-table-column label="变化上传" width="120" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-tag type="success" v-if="scope.row.changeUpload == true"> 是 </el-tag>
						<el-tag type="danger" v-else> 否 </el-tag>
					</template>
				</el-table-column>
				<el-table-column label="更新时间" align="center" show-overflow-tooltip>
					<template #default="scope">
						{{ formatDate(new Date(scope.row.updateTime), 'YYYY-mm-dd HH:MM:SS') }}
					</template>
				</el-table-column>
				<el-table-column prop="devicePropertieNum" label="属性数量" width="120" show-overflow-tooltip />


				<el-table-column label="操作" width="120" align="center" fixed="right" show-overflow-tooltip>
					<template #default="scope">
						<el-button icon="ele-Edit" size="small" text type="primary" @click="openEditDevice(scope.row)"
							v-auth="'device:update'"> 编辑 </el-button>
							
						<el-dropdown>
							<el-button icon="ele-MoreFilled" size="small" text type="primary"
								style="padding-left: 12px" />
							<template #dropdown>
								<el-dropdown-menu>
									<el-dropdown-item icon="ele-Refresh" @click="reStartDevice(scope.row)" divided
										:disabled="!auth('device:update')"> 重启设备 </el-dropdown-item>
										<el-dropdown-item icon="ele-Delete" @click="delDevice(scope.row)" divided
										:disabled="!auth('device:delete')"> 删除设备 </el-dropdown-item>
								</el-dropdown-menu>

							</template>
						</el-dropdown>
					</template>
				</el-table-column>
			</el-table>
			<el-pagination v-model:currentPage="tableParams.page" v-model:page-size="tableParams.pageSize"
				:total="tableParams.total" :page-sizes="[10, 20, 50, 100]" small background
				@size-change="handleSizeChange" @current-change="handleCurrentChange"
				layout="total, sizes, prev, pager, next, jumper" />
		</el-card>


		<el-dialog v-model="dialogVisible" :lock-scroll="false" :close-on-click-modal="false" draggable width="400px">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle">
						<ele-UploadFilled />
					</el-icon>
					<span> 上传文件 </span>
				</div>
			</template>
			<div>
				<el-upload ref="uploadRef" drag :auto-upload="false" :limit="1" :file-list="fileList" action=""
					:on-change="handleChange" accept=".jpg,.png,.bmp,.gif,.txt,.pdf,.xlsx,.docx">
					<el-icon class="el-icon--upload">
						<ele-UploadFilled />
					</el-icon>
					<div class="el-upload__text">将文件拖到此处，或<em>点击上传</em></div>
					<template #tip>
						<div class="el-upload__tip">请上传大小不超过 10MB 的文件</div>
					</template>
				</el-upload>
			</div>
			<template #footer>
				<span class="dialog-footer">
					<el-button @click="dialogVisible = false">取消</el-button>
					<el-button type="primary" @click="importDevice">确定</el-button>
				</span>
			</template>
		</el-dialog>

		<EditDevice ref="editDeviceRef" :title="editDeviceTitle" :isAdd="isAdd" />
	</div>
</template>

<script lang="ts">
import { ref, toRefs, reactive, onMounted, defineComponent, onUnmounted } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import { formatDate } from '/@/utils/formatTime';
import { auth } from '/@/utils/authFunction';
import mittBus from '/@/utils/mitt';
import EditDevice from './component/editDevice.vue';
import { downloadByData, getFileName } from '/@/utils/download';

import { getAPI } from '/@/utils/axios-utils';
import { DeviceApi} from '/@/api-services/api';
import { Device } from '/@/api-services/models';

export default defineComponent({
	name: 'device',
	components: { EditDevice },
	setup() {
		const editDeviceRef = ref();
		const state = reactive({
			loading: false,
			deviceData: [] as Array<Device>,
			queryParams: {
				name: undefined,
				driverAssembleName: undefined,
			},
			tableParams: {
				page: 1,
				pageSize: 10,
				total: 0 as any,
			},
			editDeviceTitle: '',
			isAdd: false,
			dialogVisible: false,
			fileList: [] as any,

		});
		onMounted(async () => {
			handleQuery();

			mittBus.on('deviceRefresh', () => {
				handleQuery();
			});
		});
		onUnmounted(() => {
			mittBus.off('deviceRefresh');
		});
		// 通过onChanne方法获得文件列表
		const handleChange = (file: any, fileList: []) => {
			state.fileList = fileList;
		};

		// 查询操作
		const handleQuery = async () => {
			state.loading = true;
			var res = await getAPI(DeviceApi).apiDeviceGetDevicePageGet(
				state.queryParams.name,
				state.queryParams.driverAssembleName,
				state.tableParams.page,
				state.tableParams.pageSize
			);
			state.deviceData = res.data.result?.items ?? [];
			state.tableParams.total = res.data.result?.total;
			state.loading = false;
		};
		// 重置操作
		const resetQuery = () => {
			state.queryParams.name = undefined;
			state.queryParams.driverAssembleName = undefined;
			handleQuery();
		};
		//打开新增页面
		const openAddDevice = () => {
			state.editDeviceTitle = '添加设备';
			state.isAdd = true;
			editDeviceRef.value.openDialog({});
		};
		//打开编辑页面
		const openEditDevice = (row: any) => {
			state.editDeviceTitle = '编辑设备';
			state.isAdd = false;
			editDeviceRef.value.openDialog(row);
		};
		// 删除
		const delDevice = (row: any) => {
			ElMessageBox.confirm(`确定删除设备：【${row.name}】?`, '提示', {
				confirmButtonText: '确定',
				cancelButtonText: '取消',
				type: 'warning',
			})
				.then(async () => {
					await getAPI(DeviceApi).apiDeviceDeleteConfigPost({ id: row.id });
					handleQuery();
					ElMessage.success('删除成功');
				})
				.catch(() => { });
		};
		// 重启
		const reStartDevice = (row: any) => {
			let mess=row.id==undefined?`确定全部重启?`:`确定重启设备：【${row.name}】?`
			ElMessageBox.confirm(mess, '提示', {
				confirmButtonText: '确定',
				cancelButtonText: '取消',
				type: 'warning',
			})
				.then(async () => {
					await getAPI(DeviceApi).apiDeviceRestartDevicePost({ id: row.id});
					handleQuery();
					ElMessage.success('重启成功');
				})
				.catch(() => { });
		};
		// 改变页面容量
		const handleSizeChange = (val: number) => {
			state.tableParams.pageSize = val;
			handleQuery();
		};
		// 改变页码序号
		const handleCurrentChange = (val: number) => {
			state.tableParams.page = val;
			handleQuery();
		};
		//修改状态
		const changeStatus = async (row: any) => {
			await getAPI(DeviceApi)
				.apiDeviceConfigDeviceStatusPost({ id: row.id, invokeEnable: row.invokeEnable })
				.catch(() => {
					row.invokeEnable = row.invokeEnable == true ? false : true;
				});
		};
		//下载


		// 导出设备
		const exportDevice = async () => {
			state.loading = true;
			var res = await getAPI(DeviceApi).apiDeviceExportFilePost(state.queryParams, { responseType: 'blob' });
			state.loading = false;

			var fileName = getFileName(res.headers);
			downloadByData(res.data as any, fileName);
		};
		//导入设备
		const importDevice = async () => {
			if (state.fileList.length < 1) return;
			await getAPI(DeviceApi).apiDeviceImprotFilePostForm(state.fileList[0].raw);
			handleQuery();
			ElMessage.success('上传成功');
			state.dialogVisible = false;
		};


		return {
			handleQuery,
			resetQuery,

			editDeviceRef,

			openAddDevice,
			exportDevice,
			importDevice,
			openEditDevice,
			delDevice,
			reStartDevice,

			handleChange,
			changeStatus,
			handleSizeChange,
			handleCurrentChange,
			formatDate,
			auth,
			...toRefs(state),
		};
	},
});
</script>
