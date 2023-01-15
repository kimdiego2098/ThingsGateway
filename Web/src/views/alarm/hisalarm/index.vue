<template>
	<div class="sys-hisalarm-container">
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
				<el-table-column prop="deviceName" label="设备名称" width="120" fixed show-overflow-tooltip>
				</el-table-column>
				<el-table-column prop="coreDataType" label="数据类型" width="90" fixed show-overflow-tooltip>
					<template #default="scope">
						{{ dataTypeFormat(scope.row.coreDataType) }}
					</template>
				</el-table-column>

				<el-table-column label="报警时间" width="130" align="center" show-overflow-tooltip>
					<template #default="scope">
						{{ formatDate(new Date(scope.row.variableAlarmsAlarmTime), 'mm-dd HH:MM:SS') }}
					</template>
				</el-table-column>

				<el-table-column prop="variableAlarmsAlarmType" label="报警类型" width="100" align="center" fixed show-overflow-tooltip>
					<template #default="scope">
						<el-tag effect="plain" v-if="scope.row.variableAlarmsAlarmType === 1">开报警</el-tag>
						<el-tag effect="plain" v-else-if="scope.row.variableAlarmsAlarmType === 2">关报警</el-tag>
						<el-tag effect="plain" v-else-if="scope.row.variableAlarmsAlarmType === 3">高高报警</el-tag>
						<el-tag effect="plain" v-else-if="(scope.row.variableAlarmsAlarmType === 4)">高报警</el-tag>
						<el-tag effect="plain" v-else-if="(scope.row.variableAlarmsAlarmType === 5)">低报警</el-tag>
						<el-tag effect="plain" v-else-if="(scope.row.variableAlarmsAlarmType === 6)">低低报警</el-tag>
					</template>
				</el-table-column>

				<el-table-column prop="variableAlarmsAlarmCode" label="报警/恢复值" width="120" align="center" fixed show-overflow-tooltip>
					<template #default="scope">
						<el-tag type="success"> {{ scope.row.variableAlarmsAlarmCode }}</el-tag>
					</template>
				</el-table-column>

				<el-table-column prop="variableAlarmsAlarmLimit" label="报警限值" width="100" align="center" fixed show-overflow-tooltip>
					<template #default="scope">
						<el-tag type="success"> {{ scope.row.variableAlarmsAlarmLimit }}</el-tag>
					</template>
				</el-table-column>



				<el-table-column label="事件时间" width="130" align="center" show-overflow-tooltip>
					<template #default="scope">
						{{ formatDate(new Date(scope.row.variableAlarmsEventTime), 'mm-dd HH:MM:SS') }}
					</template>
				</el-table-column>

				<el-table-column prop="value" label="事件类型" width="100" align="center" fixed show-overflow-tooltip>
					<template #default="scope">
						<el-tag effect="plain" v-if="scope.row.variableAlarmsEventType === 1">报警</el-tag>
						<el-tag effect="plain" v-else-if="scope.row.variableAlarmsEventType === 2">确认</el-tag>
						<el-tag effect="success" v-else-if="scope.row.variableAlarmsEventType === 3">恢复</el-tag>
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

import { getAPI } from '/@/utils/axios-utils';
import { AlarmRunTimeApi } from '/@/api-services/api';
import { AlarmHis } from '/@/api-services/models';

import { dataTypeList, protectTypeList } from '../../collect/variable/variable';



export default defineComponent({
	name: 'device',
	components: {},
	setup() {
		const editVariableRef = ref();
		const state = reactive({
			loading: false,
			deviceVariableData: [] as Array<AlarmHis>,

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

		});


		onMounted(async () => {
			handleQuery();

		});
		onUnmounted(() => {

		});

		// 查询操作
		const handleQuery = async () => {
			state.loading = true;
			var res = await getAPI(AlarmRunTimeApi).apiAlarmRunTimeGetHisAlarmPageGet(
				state.queryParams.name,
				state.queryParams.deviceName,
				state.tableParams.page,
				state.tableParams.pageSize
			);
			state.deviceVariableData = res.data.result?.items ?? [];
			state.tableParams.total = res.data.result?.total;


			state.loading = false;
		};
		// 重置操作
		const resetQuery = () => {
			state.queryParams.name = undefined;
			state.queryParams.deviceName = undefined;
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
