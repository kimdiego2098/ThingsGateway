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

				<el-form-item label="变量描述" prop="name">
					<el-input placeholder="变量描述" clearable @keyup.enter="handleQuery" v-model="queryParams.des" />
				</el-form-item>

				<el-form-item label="质量戳" prop="name">
					<el-input placeholder="质量戳" clearable @keyup.enter="handleQuery" v-model="queryParams.qua" />
				</el-form-item>


				<el-form-item label="执行参数" prop="name">
					<el-input placeholder="执行参数" clearable @keyup.enter="handleQuery" v-model="queryParams.address" />
				</el-form-item>

				<el-form-item>
					<el-button icon="ele-Refresh" @click="resetQuery"> 重置 </el-button>
					<el-button type="primary" icon="ele-Search" @click="handleQuery" v-auth="'variable:page'"> 查询
					</el-button>
				</el-form-item>
			</el-form>
		</el-card>

		<el-card shadow="hover" style="margin-top: 8px">
			<el-table :data="deviceVariableData" style="width: 100%" v-loading="loading" border>
				<el-table-column type="index" label="序号" width="55" align="center" fixed />
				<el-table-column prop="name" label="变量名称" width="120" fixed show-overflow-tooltip />
				<el-table-column prop="description" label="描述" width="120" fixed show-overflow-tooltip />
				<el-table-column prop="deviceId" label="设备名称" width="120" fixed show-overflow-tooltip>
					<template #default="scope">
						{{ devIdNameFormat(scope.row.deviceId) }}
					</template>
				</el-table-column>
				<el-table-column prop="coreDataType" label="数据类型" width="100" fixed show-overflow-tooltip>
					<template #default="scope">
						{{ dataTypeFormat(scope.row.coreDataType) }}
					</template>
				</el-table-column>

				<el-table-column prop="value" label="实时值" align="center" fixed show-overflow-tooltip>
					<template #default="scope">
						<el-tag type="success"> {{ scope.row.value }}</el-tag>
					</template>
				</el-table-column>

				<el-table-column prop="rawValue" label="原始值" fixed show-overflow-tooltip>
					<template #default="scope">
						<el-tag type="success"> {{ scope.row.rawValue }}</el-tag>
					</template>
				</el-table-column>

				<el-table-column prop="quality" label="质量戳" width="70" fixed show-overflow-tooltip>
					<template #default="scope">
						<el-tag type="success" v-if="scope.row.quality == 192"> 成功</el-tag>
						<el-tag type="danger" v-else> 失败</el-tag>
					</template>
				</el-table-column>

				<el-table-column label="采集时间" fixed width="130" align="center" show-overflow-tooltip>
					<template #default="scope">
						{{ formatDate(new Date(scope.row.collectTime), 'mm-dd HH:MM:SS') }}
					</template>
				</el-table-column>
				<el-table-column label="变化时间" width="130" align="center" show-overflow-tooltip>
					<template #default="scope">
						{{ formatDate(new Date(scope.row.changeTime), 'mm-dd HH:MM:SS') }}
					</template>
				</el-table-column>

				<el-table-column prop="initialValue" label="初始值" show-overflow-tooltip />

				<el-table-column prop="variableAddress" label="执行参数" width="120" show-overflow-tooltip />
				<el-table-column prop="otherMethod" label="其他方法" width="120" show-overflow-tooltip />
				<el-table-column prop="expressions" label="表达式" width="120" show-overflow-tooltip />

				<el-table-column prop="invokeInterval" label="执行间隔" width="120" show-overflow-tooltip />

				<el-table-column prop="protectType" label="读写权限" width="120" show-overflow-tooltip>
					<template #default="scope">
						{{ protectTypeFormat(scope.row.protectType) }}
					</template>
				</el-table-column>

				<el-table-column label="操作" width="110" align="center" fixed="right" show-overflow-tooltip>
					<template #default="scope">
						<el-button icon="ele-Edit" size="small" text type="primary" @click="writeVariable(scope.row)"
							v-auth="'variable:update'"> 写入变量 </el-button>
					</template>
				</el-table-column>

			</el-table>
			<el-pagination v-model:currentPage="tableParams.page" v-model:page-size="tableParams.pageSize"
				:total="tableParams.total" :page-sizes="[10, 20, 50, 100]" small background
				@size-change="handleSizeChange" @current-change="handleCurrentChange"
				layout="total, sizes, prev, pager, next, jumper" />
		</el-card>

	</div>
</template>

<script lang="ts">
import { ref, toRefs, reactive, onMounted, defineComponent, onUnmounted } from 'vue';
import { formatDate } from '/@/utils/formatTime';
import { auth } from '/@/utils/authFunction';
import mittBus from '/@/utils/mitt';

import { getAPI } from '/@/utils/axios-utils';
import { DeviceApi, DeviceVariableRunTimeApi } from '/@/api-services/api';
import { DeviceVariable, DevIdName } from '/@/api-services/models';

import { dataTypeList, protectTypeList } from '../../collect/variable/variable';
import { ElMessageBox, ElMessage } from 'element-plus';

//mqtt websocket
import useMqtt from '../../tgutils/usemqtt';


export default defineComponent({
	name: 'device',
	components: {},
	setup() {
		const editVariableRef = ref();
		const state = reactive({
			loading: false,
			deviceVariableData: [] as Array<DeviceVariable>,

			queryParams: {
				group: -1,
				name: undefined,
				deviceName: undefined,

				des: undefined,
				qua: undefined,
				address: undefined,

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
			deviceNameList: [] as Array<DevIdName>,

	
		});
		let newVariables = [] as Array<DeviceVariable>;
		const { startMqtt, onMqttUnmounted } = useMqtt();
		startMqtt("device/variablestatus/#", (topic: any, message: any) => {
			newVariables = JSON.parse(message.toString())
			newVariables.forEach(a => {
				state.deviceVariableData.forEach(b => {
					if (a.id == b.id) {
						b.value = a.value
						b.rawValue = a.rawValue
						b.changeTime = a.changeTime
						b.collectTime = a.collectTime
						b.quality = a.quality
					}
				})
			})
			state.deviceVariableData.push()

		});

		onMounted(async () => {
			handleQuery();
			mittBus.on('deviceVariableRefresh', () => {
				handleQuery();
			});
		});
		onUnmounted(() => {

			mittBus.off('deviceVariableRefresh');
			onMqttUnmounted();
		});

		// 通过onChanne方法获得文件列表
		const handleChange = (file: any, fileList: []) => {
			state.fileList = fileList;
		};
		// 查询操作
		const handleQuery = async () => {
			state.loading = true;
			var res = await getAPI(DeviceVariableRunTimeApi).apiDeviceVariableRunTimeGetDeviceVariablePageGet(
				state.queryParams.name,
				state.queryParams.deviceName,
				state.queryParams.des,
				state.queryParams.qua,
				state.queryParams.address,
				state.tableParams.page,
				state.tableParams.pageSize
			);
			state.deviceVariableData = res.data.result?.items ?? [];
			state.tableParams.total = res.data.result?.total;

			let resDicData = await getAPI(DeviceApi).apiDeviceGetDeviceNamesGet();
			state.deviceNameList = resDicData.data.result ?? [];

			state.loading = false;
		};
		// 重置操作
		const resetQuery = () => {
			state.queryParams.name = undefined;
			state.queryParams.deviceName = undefined;
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
		// 写入
		const writeVariable = (row: any) => {
			ElMessageBox.prompt(`写入变量：【${row.name}】?`, '提示', {
				confirmButtonText: '确定',
				cancelButtonText: '取消',
				type: 'warning',
			})
				.then(async (value1) => {
					var data=	await getAPI(DeviceVariableRunTimeApi).apiDeviceVariableRunTimeWriteDeviceVariableValuePost({name: row.name,writeValue:value1.value});
					handleQuery();
					ElMessage.success(data.data.result as string);

				})
				.catch(() => { });
		};
		return {
			handleQuery,
			resetQuery,
			editVariableRef,
			dataTypeList,
			protectTypeList,
			writeVariable,
			devIdNameFormat,
			handleChange,
			dataTypeFormat,
			protectTypeFormat,
			handleSizeChange,
			handleCurrentChange,
			formatDate,
			auth,
			...toRefs(state),
		};
	},
});
</script>
