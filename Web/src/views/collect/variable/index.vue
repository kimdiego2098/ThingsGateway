<template>
	<div class="sys-variable-container">
		<el-card shadow="hover" :body-style="{ paddingBottom: '0' }">
			<el-form :model="queryParams" ref="queryForm" :inline="true">
				<el-form-item label="变量名称" prop="name">
					<el-input placeholder="变量名称" clearable @keyup.enter="handleQuery" v-model="queryParams.name" />
				</el-form-item>
				<el-form-item label="设备名称" prop="driverAssembleName">
					<el-input placeholder="设备名称" clearable @keyup.enter="handleQuery"
						v-model="queryParams.deviceName" />
				</el-form-item>
				


				<el-form-item>
					<el-button icon="ele-Refresh" @click="resetQuery"> 重置 </el-button>
					<el-button type="primary" icon="ele-Search" @click="handleQuery" v-auth="'variable:page'"> 查询
					</el-button>
					<el-button icon="ele-Plus" @click="openAddVariable" v-auth="'variable:add'"> 新增 </el-button>
					<el-button icon="ele-FolderOpened" @click="exportVariable" v-auth="'variable:export'"> 导出 </el-button>
					<el-button icon="ele-UploadFilled" @click="dialogVisible = true" v-auth="'variable:import'"> 导入
					</el-button>
				</el-form-item>
			</el-form>
		</el-card>

		<el-card shadow="hover" style="margin-top: 8px">
			<el-table :data="deviceVariableData" style="width: 100%" v-loading="loading" border>
				<el-table-column type="index" label="序号" width="55" align="center" fixed />
				<el-table-column prop="name" label="变量名称" width="120" fixed show-overflow-tooltip />
				<el-table-column prop="description" label="描述" width="250" show-overflow-tooltip />
				<el-table-column prop="deviceId" label="设备名称" width="120"  fixed show-overflow-tooltip>
					<template #default="scope">
						{{ devIdNameFormat(scope.row.deviceId) }}
					</template>
				</el-table-column>

				<el-table-column prop="coreDataType" label="数据类型" width="120" fixed show-overflow-tooltip>
					<template #default="scope">
						{{ dataTypeFormat(scope.row.coreDataType) }}
					</template>
				</el-table-column>
				<el-table-column prop="initialValue" label="初始值" show-overflow-tooltip />
				<el-table-column prop="protectType" label="读写权限" width="120" show-overflow-tooltip>
					<template #default="scope">
						{{ protectTypeFormat(scope.row.protectType) }}
					</template>
				</el-table-column>
				
				<el-table-column prop="variableAddress" label="执行参数" width="120" show-overflow-tooltip />
				<el-table-column prop="otherMethod" label="其他方法" width="120" show-overflow-tooltip />
				<el-table-column prop="expressions" label="表达式" width="120" show-overflow-tooltip />
				<el-table-column prop="invokeInterval" label="执行间隔" width="120" show-overflow-tooltip />

				<el-table-column label="更新时间" width="120" align="center" show-overflow-tooltip>
					<template #default="scope">
						{{ formatDate(new Date(scope.row.updateTime), 'YYYY-mm-dd HH:MM:SS') }}
					</template>
				</el-table-column>


				<el-table-column label="操作" width="110" align="center" fixed="right" show-overflow-tooltip>
					<template #default="scope">
						<el-button icon="ele-Edit" size="small" text type="primary" @click="openEditVariable(scope.row)"
							v-auth="'variable:update'"> 编辑 </el-button>
						<el-dropdown>
							<el-button icon="ele-MoreFilled" size="small" text type="primary"
								style="padding-left: 12px" />
							<template #dropdown>
								<el-dropdown-menu>
									<el-dropdown-item icon="ele-Delete" @click="delVariable(scope.row)" divided
										:disabled="!auth('variable:delete')"> 删除变量 </el-dropdown-item>
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
					<el-button type="primary" @click="importVariable">确定</el-button>
				</span>
			</template>
		</el-dialog>


		<EditVariable ref="editVariableRef" :title="editVariableTitle" :is-add="isAdd" />
	</div>
</template>

<script lang="ts">
import { ref, toRefs, reactive, onMounted, defineComponent, onUnmounted } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import { formatDate } from '/@/utils/formatTime';
import { auth } from '/@/utils/authFunction';
import mittBus from '/@/utils/mitt';
import EditVariable from './component/editVariable.vue';
import { downloadByData, getFileName } from '/@/utils/download';

import { getAPI } from '/@/utils/axios-utils';
import { DeviceApi,DeviceVariableApi } from '/@/api-services/api';
import { DeviceVariable,DevIdName } from '/@/api-services/models';
import { dataTypeList, protectTypeList } from './variable';

export default defineComponent({
	name: 'device',
	components: { EditVariable },
	setup() {
		const editVariableRef = ref();
		const state = reactive({
			loading: false,
			deviceVariableData: [] as Array<DeviceVariable>,

			queryParams: {
				group: -1,
				name: undefined,
				deviceName: undefined,

			},
			tableParams: {
				page: 1,
				pageSize: 10,
				total: 0 as any,
			},
			editVariableTitle: '',
			isAdd: false,

			dialogVisible: false,
			fileList: [] as any,
			deviceNameList:  [] as Array<DevIdName>,


		});
		onMounted(async () => {
			handleQuery();

			mittBus.on('deviceVariableRefresh', () => {
				handleQuery();
			});
		});
		onUnmounted(() => {
			mittBus.off('deviceVariableRefresh');
		});

		// 通过onChanne方法获得文件列表
		const handleChange = (file: any, fileList: []) => {
			state.fileList = fileList;
		};
		// 查询操作
		const handleQuery = async () => {
			state.loading = true;
			var res = await getAPI(DeviceVariableApi).apiDeviceVariableGetDeviceVariablePageGet(
				state.queryParams.name,
				state.queryParams.deviceName,
				state.tableParams.page,
				state.tableParams.pageSize
			);
			state.deviceVariableData = res.data.result?.items ?? [];
			state.tableParams.total = res.data.result?.total;

			
			let resDicData = await getAPI(DeviceApi).apiDeviceGetDeviceNamesGet();
			state.deviceNameList = resDicData.data.result??[];


			state.loading = false;
		};
		// 重置操作
		const resetQuery = () => {
			state.queryParams.name = undefined;
			state.queryParams.deviceName = undefined;
			handleQuery();
		};
		//打开新增页面
		const openAddVariable = () => {
			state.editVariableTitle = '添加变量';
			state.isAdd = true;
			editVariableRef.value.openDialog({});
		};
		//打开编辑页面
		const openEditVariable = (row: any) => {
			state.editVariableTitle = '编辑变量';
			state.isAdd = false;
			editVariableRef.value.openDialog(row);
		};
		// 删除
		const delVariable = (row: any) => {
			ElMessageBox.confirm(`确定删除变量：【${row.name}】?`, '提示', {
				confirmButtonText: '确定',
				cancelButtonText: '取消',
				type: 'warning',
			})
				.then(async () => {
					await getAPI(DeviceVariableApi).apiDeviceVariableDeleteVariableConfigPost({ id: row.id });
					handleQuery();
					ElMessage.success('删除成功');
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
		const devIdNameFormat = (row: number) => {
			let branchName
			state.deviceNameList.forEach(item => {
				if (item.id == row) {
					branchName = item.name
				}
			})
			return branchName
		};
		const dataTypeFormat = (row: number) => {
			let branchName = ''
			dataTypeList.forEach(item => {
				if (item.value == row) {
					branchName = item.label
				}
			})
			return branchName
		};
		const protectTypeFormat = (row: number) => {
			let branchName = ''
			protectTypeList.forEach(item => {
				if (item.value == row) {
					branchName = item.label
				}
			})
			return branchName
		};
		//导出设备变量
		const exportVariable = async () => {
			state.loading = true;
			var res = await getAPI(DeviceVariableApi).apiDeviceVariableExportFilePost(state.queryParams, { responseType: 'blob' });
			state.loading = false;

			var fileName = getFileName(res.headers);
			downloadByData(res.data as any, fileName);
		};
		//导入设备变量
		const importVariable = async () => {
			if (state.fileList.length < 1) return;
			await getAPI(DeviceVariableApi).apiDeviceVariableImprotFilePostForm(state.fileList[0].raw);
			handleQuery();
			ElMessage.success('上传成功');
			state.dialogVisible = false;
		};

		return {
			handleQuery,
			resetQuery,

			editVariableRef,
			dataTypeList,
			protectTypeList,

			openAddVariable,
			openEditVariable,
			delVariable,

			exportVariable,
			importVariable,

			dataTypeFormat,
			protectTypeFormat,
			devIdNameFormat,

			handleChange,
			handleSizeChange,
			handleCurrentChange,
			formatDate,
			auth,
			...toRefs(state),
		};
	},
});
</script>
