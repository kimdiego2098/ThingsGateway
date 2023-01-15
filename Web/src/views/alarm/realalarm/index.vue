<template>
	<div class="sys-alarm-container">
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
				</el-form-item>
			</el-form>
		</el-card>

		<el-card shadow="hover" style="margin-top: 8px">
			<el-table :data="deviceVariableData" style="width: 100%" v-loading="loading" border>
				<el-table-column type="index" label="序号" width="55" align="center" fixed />
				<el-table-column prop="name" label="变量名称" width="120" fixed show-overflow-tooltip />
				<el-table-column prop="description" label="描述" width="100" fixed show-overflow-tooltip />
				<el-table-column prop="deviceId" label="设备名称" width="120" fixed show-overflow-tooltip>
					<template #default="scope">
						{{ devIdNameFormat(scope.row.deviceId) }}
					</template>
				</el-table-column>
				<el-table-column prop="coreDataType" label="数据类型" width="90" fixed show-overflow-tooltip>
					<template #default="scope">
						{{ dataTypeFormat(scope.row.coreDataType) }}
					</template>
				</el-table-column>

				<el-table-column label="报警时间" width="130" align="center" show-overflow-tooltip>
					<template #default="scope">
						{{ formatDate(new Date(scope.row.variableAlarms.alarmTime), 'mm-dd HH:MM:SS') }}
					</template>
				</el-table-column>

				<el-table-column prop="variableAlarms.alarmType" label="报警类型" width="100" align="center" fixed show-overflow-tooltip>
					<template #default="scope">
						<el-tag effect="plain" v-if="scope.row.variableAlarms.alarmType === 1">开报警</el-tag>
						<el-tag effect="plain" v-else-if="scope.row.variableAlarms.alarmType === 2">关报警</el-tag>
						<el-tag effect="plain" v-else-if="scope.row.variableAlarms.alarmType === 3">高高报警</el-tag>
						<el-tag effect="plain" v-else-if="(scope.row.variableAlarms.alarmType === 4)">高报警</el-tag>
						<el-tag effect="plain" v-else-if="(scope.row.variableAlarms.alarmType === 5)">低报警</el-tag>
						<el-tag effect="plain" v-else-if="(scope.row.variableAlarms.alarmType === 6)">低低报警</el-tag>
					</template>
				</el-table-column>

				<el-table-column prop="variableAlarms.alarmCode" label="报警值" width="100" align="center" fixed show-overflow-tooltip>
					<template #default="scope">
						<el-tag type="success"> {{ scope.row.variableAlarms.alarmCode }}</el-tag>
					</template>
				</el-table-column>

				<el-table-column prop="variableAlarms.alarmLimit" label="报警限值" width="100" align="center" fixed show-overflow-tooltip>
					<template #default="scope">
						<el-tag type="success"> {{ scope.row.variableAlarms.alarmLimit }}</el-tag>
					</template>
				</el-table-column>



				<el-table-column label="事件时间" width="130" align="center" show-overflow-tooltip>
					<template #default="scope">
						{{ formatDate(new Date(scope.row.variableAlarms.eventTime), 'mm-dd HH:MM:SS') }}
					</template>
				</el-table-column>

				<el-table-column prop="value" label="事件类型" width="100" align="center" fixed show-overflow-tooltip>
					<template #default="scope">
						<el-tag effect="plain" v-if="scope.row.variableAlarms.eventType === 1">报警</el-tag>
						<el-tag effect="plain" v-else-if="scope.row.variableAlarms.eventType === 2">确认</el-tag>
						<el-tag effect="plain" v-else-if="scope.row.variableAlarms.eventType === 3">恢复</el-tag>
					</template>
				</el-table-column>

				<el-table-column prop="value" label="实时值" align="center" fixed show-overflow-tooltip>
					<template #default="scope">
						<el-tag type="success"> {{ scope.row.value }}</el-tag>
					</template>
				</el-table-column>

				<el-table-column prop="quality" label="质量戳" width="70" fixed show-overflow-tooltip>
					<template #default="scope">
						<el-tag type="success" v-if="scope.row.quality == 192"> 成功</el-tag>
						<el-tag type="danger" v-else> 失败</el-tag>
					</template>
				</el-table-column>

				<el-table-column label="采集时间"  width="130" align="center" show-overflow-tooltip>
					<template #default="scope">
						{{ formatDate(new Date(scope.row.collectTime), 'mm-dd HH:MM:SS') }}
					</template>
				</el-table-column>
				<el-table-column label="变化时间" width="130" align="center" show-overflow-tooltip>
					<template #default="scope">
						{{ formatDate(new Date(scope.row.changeTime), 'mm-dd HH:MM:SS') }}
					</template>
				</el-table-column>



			</el-table>
			<el-pagination v-model:currentPage="tableParams.page" v-model:page-size="tableParams.pageSize"
				:total="tableParams.total" :page-sizes="[100]" small background
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
import { DeviceApi, AlarmRunTimeApi } from '/@/api-services/api';
import { DeviceVariable, DevIdName } from '/@/api-services/models';

import { dataTypeList, protectTypeList } from '../../collect/variable/variable';

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
			},
			tableParams: {
				page: 1,
				pageSize: 100,
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
		startMqtt("device/alarmstatus", (topic:any, message:any) => {

			newVariables = JSON.parse(message.toString())
			state.deviceVariableData = newVariables;
			// state.deviceVariableData.push()

		});

		onMounted(async () => {
			handleQuery();

		});
		onUnmounted(() => {
			onMqttUnmounted();
		});

		// 通过onChanne方法获得文件列表
		const handleChange = (file: any, fileList: []) => {
			state.fileList = fileList;
		};
		// 查询操作
		const handleQuery = async () => {
			state.loading = true;
			var res = await getAPI(AlarmRunTimeApi).apiAlarmRunTimeGetRealAlarmPageGet(
				state.queryParams.name,
				state.queryParams.deviceName,
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

		return {
			handleQuery,
			resetQuery,
			editVariableRef,
			dataTypeList,
			protectTypeList,

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
