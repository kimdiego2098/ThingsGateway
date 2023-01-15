<template>
	<div class="sys-exlog-container" v-loading="loading">
		<el-card shadow="hover" :body-style="{ paddingBottom: '0' }">
			<el-form :model="queryParams" ref="queryForm" :inline="true">
				<el-form-item label="开始时间" prop="name">
					<el-date-picker v-model="queryParams.startTime" type="datetime" placeholder="开始时间" :shortcuts="shortcuts" />
				</el-form-item>
				<el-form-item label="结束时间" prop="code">
					<el-date-picker v-model="queryParams.endTime" type="datetime" placeholder="结束时间" :shortcuts="shortcuts" />
				</el-form-item>
				<el-form-item>
					<el-button icon="ele-Refresh" @click="resetQuery"> 重置 </el-button>
					<el-button type="primary" icon="ele-Search" @click="handleQuery" v-auth="'sysExlog:page'"> 查询 </el-button>
					<el-button icon="ele-DeleteFilled" type="danger" @click="clearLog" v-auth="'sysExlog:clear'"> 清空 </el-button>
					<el-button icon="ele-FolderOpened" @click="exportLog" v-auth="'sysExlog:export'"> 导出 </el-button>
				</el-form-item>
			</el-form>
		</el-card>

		<el-card shadow="hover" style="margin-top: 8px">
			<el-table :data="logData" style="width: 100%" border>
				<el-table-column type="index" label="序号" width="55" align="center" />
				<el-table-column prop="logName" label="类别名称" show-overflow-tooltip />
				<el-table-column prop="logLevel" label="日志级别" width="100" align="center" show-overflow-tooltip />
				<el-table-column prop="eventId" label="事件Id" width="70" align="center" show-overflow-tooltip />
				<el-table-column prop="message" label="日志消息" show-overflow-tooltip />
				<el-table-column prop="exception" label="异常对象" show-overflow-tooltip />
				<el-table-column prop="state" label="当前状态值" show-overflow-tooltip />
				<el-table-column prop="threadId" label="线程Id" width="70" align="center" show-overflow-tooltip />
				<el-table-column prop="traceId" label="请求跟踪Id" show-overflow-tooltip />
				<el-table-column prop="logDateTime" label="记录时间" align="center" show-overflow-tooltip />
				<!-- <el-table-column prop="createTime" label="操作时间" align="center" show-overflow-tooltip /> -->
			</el-table>
			<el-pagination
				v-model:currentPage="tableParams.page"
				v-model:page-size="tableParams.pageSize"
				:total="tableParams.total"
				:page-sizes="[10, 20, 50, 100]"
				small
				background
				@size-change="handleSizeChange"
				@current-change="handleCurrentChange"
				layout="total, sizes, prev, pager, next, jumper"
			/>
		</el-card>
	</div>
</template>

<script lang="ts">
import { toRefs, reactive, onMounted, defineComponent } from 'vue';
import { ElMessage } from 'element-plus';
import { downloadByData, getFileName } from '/@/utils/download';

import { getAPI } from '/@/utils/axios-utils';
import { SysLogExApi } from '/@/api-services/api';
import { SysLogEx } from '/@/api-services/models';

export default defineComponent({
	name: 'sysExLog',
	components: {},
	setup() {
		const state = reactive({
			loading: false,
			queryParams: {
				startTime: undefined,
				endTime: undefined,
			},
			tableParams: {
				page: 1,
				pageSize: 10,
				total: 0 as any,
			},
			logData: [] as Array<SysLogEx>,
		});
		onMounted(async () => {
			handleQuery();
		});
		// 查询操作
		const handleQuery = async () => {
			if (state.queryParams.startTime == null) state.queryParams.startTime = undefined;
			if (state.queryParams.endTime == null) state.queryParams.endTime = undefined;
			state.loading = true;
			var res = await getAPI(SysLogExApi).apiSysLogExGetLogExPageGet(state.queryParams.startTime, state.queryParams.endTime, state.tableParams.page, state.tableParams.pageSize);
			state.logData = res.data.result?.items ?? [];
			state.tableParams.total = res.data.result?.total;
			state.loading = false;
		};
		// 重置操作
		const resetQuery = () => {
			state.queryParams.startTime = undefined;
			state.queryParams.endTime = undefined;
			handleQuery();
		};
		// 清空日志
		const clearLog = async () => {
			state.loading = true;
			await getAPI(SysLogExApi).apiSysLogExClearLogExPost();
			state.loading = false;

			ElMessage.success('清空成功');
			handleQuery();
		};
		// 导出日志
		const exportLog = async () => {
			state.loading = true;
			var res = await getAPI(SysLogExApi).apiSysLogExExporLogOpPost(state.queryParams, { responseType: 'blob' });
			state.loading = false;

			var fileName = getFileName(res.headers);
			downloadByData(res.data as any, fileName);
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
		const shortcuts = [
			{
				text: '今天',
				value: new Date(),
			},
			{
				text: '昨天',
				value: () => {
					const date = new Date();
					date.setTime(date.getTime() - 3600 * 1000 * 24);
					return date;
				},
			},
			{
				text: '上周',
				value: () => {
					const date = new Date();
					date.setTime(date.getTime() - 3600 * 1000 * 24 * 7);
					return date;
				},
			},
		];
		return {
			handleQuery,
			resetQuery,
			clearLog,
			exportLog,
			shortcuts,
			handleSizeChange,
			handleCurrentChange,
			...toRefs(state),
		};
	},
});
</script>

<style lang="scss">
.el-popper {
	max-width: 60%;
}
</style>
