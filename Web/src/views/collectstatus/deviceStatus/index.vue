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

				</el-form-item>
			</el-form>
		</el-card>

		<el-card shadow="hover" style="margin-top: 8px">
			<el-table :data="deviceData" style="width: 100%" v-loading="loading" border>
				<el-table-column type="index" label="序号" width="55" align="center" fixed />
				<el-table-column prop="name" label="设备名称" width="120" fixed show-overflow-tooltip />
				<el-table-column prop="description" label="描述" show-overflow-tooltip />
				<el-table-column prop="driverAssembleName" label="驱动名称" width="150" show-overflow-tooltip />

				<el-table-column label="启用" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-tag type="success" v-if="scope.row.invokeEnable == true"> 是 </el-tag>
						<el-tag type="danger" v-else> 否 </el-tag>
					</template>
				</el-table-column>



				<el-table-column prop="deviceStatus.deviceOnLineStatus" label="在线状态" width="120" show-overflow-tooltip>
					<template #default="scope">
						<el-tag type="success" v-if="scope.row.deviceStatus.deviceOnLineStatus == 0">
							{{ DeviceOnLineStatusList(scope.row.deviceStatus.deviceOnLineStatus) }}
						</el-tag>
						<el-tag type="danger" v-if="scope.row.deviceStatus.deviceOnLineStatus != 0">
							{{ DeviceOnLineStatusList(scope.row.deviceStatus.deviceOnLineStatus) }}
						</el-tag>
					</template>
				</el-table-column>

				<el-table-column label="活跃时间" align="center" show-overflow-tooltip>
					<template #default="scope">
						{{ formatDate(new Date(scope.row.deviceStatus.activeTime), 'mm-dd HH:MM:SS') }}
					</template>
				</el-table-column>


				<el-table-column label="分包数量" width="120" align="center" show-overflow-tooltip>

					<template #default="scope">
						<el-tag type="success">
							{{ (scope.row.deviceStatus.deviceSourceVariableNum +
									scope.row.deviceStatus.deviceMedsVariableNum)
							}}
						</el-tag>
					</template>

				</el-table-column>
				<el-table-column label="失败数量" width="120" align="center" show-overflow-tooltip>

					<template #default="scope">
						<el-tag type="success">
							{{ (scope.row.deviceStatus.deviceMedsVariableFailedNum +
									scope.row.deviceStatus.deviceSourceVariableFailedNum)
							}}
						</el-tag>
					</template>

				</el-table-column>

				<el-table-column prop="invokeInterval" label="读取间隔" width="120" show-overflow-tooltip />

				<el-table-column prop="deviceVariableNum" label="变量数量" width="120" show-overflow-tooltip />

				<el-table-column label="变化上传" width="120" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-tag type="success" v-if="scope.row.changeUpload == true"> 是 </el-tag>
						<el-tag type="danger" v-else> 否 </el-tag>
					</template>
				</el-table-column>


			</el-table>
			<el-pagination v-model:currentPage="tableParams.page" v-model:page-size="tableParams.pageSize"
				:total="tableParams.total" :page-sizes="[10, 20, 50, 100]" small background
				@size-change="handleSizeChange" @current-change="handleCurrentChange"
				layout="total, sizes, prev, pager, next, jumper" />
		</el-card>
		<!-- </el-col> -->

		<!-- </el-row> -->
	</div>
</template>

<script lang="ts">
import { ref, toRefs, reactive, onMounted, defineComponent, onUnmounted } from 'vue';
import { formatDate } from '/@/utils/formatTime';
import { auth } from '/@/utils/authFunction';
import mittBus from '/@/utils/mitt';
import { deviceOnLineStatusList } from './device';

import { getAPI } from '/@/utils/axios-utils';
import { DeviceRunTimeApi } from '/@/api-services/api';
import { Device } from '/@/api-services/models';

import useMqtt from '../../tgutils/usemqtt';

export default defineComponent({
	name: 'device',
	components: {},
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

		let newDevs = {} as Device;
		const { startMqtt, onMqttUnmounted } = useMqtt();
		startMqtt("device/devicestatus/#", (topic:any, message:any) => {
			newDevs = JSON.parse(message.toString())
			state.deviceData.forEach(b => {
				if (b.id == newDevs.id) {
					b.deviceStatus = newDevs.deviceStatus
				}
			})
			state.deviceData.push()

		});

		onMounted(async () => {
			// loadOrgData();
			handleQuery();

			mittBus.on('deviceRefresh', () => {
				handleQuery();
			});
		});
		onUnmounted(() => {
			mittBus.off('deviceRefresh');

			onMqttUnmounted();

		});




		// 通过onChanne方法获得文件列表
		const handleChange = (file: any, fileList: []) => {
			state.fileList = fileList;
		};
		const DeviceOnLineStatusList = (row: number) => {
			let branchName = ''
			deviceOnLineStatusList.forEach(item => {
				if (item.value == row) {
					branchName = item.label
				}
			})
			return branchName
		};
		// 查询操作
		const handleQuery = async () => {
			state.loading = true;
			var res = await getAPI(DeviceRunTimeApi).apiDeviceRunTimeGetDevicePageGet(
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

		return {
			handleQuery,
			resetQuery,
			editDeviceRef,
			handleChange, DeviceOnLineStatusList,
			handleSizeChange,
			handleCurrentChange,
			formatDate,
			auth,
			...toRefs(state),
		};
	},
});
</script>
