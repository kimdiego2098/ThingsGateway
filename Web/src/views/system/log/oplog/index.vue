<template>
	<div class="sys-oplog-container" v-loading="loading">
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
					<el-button type="primary" icon="ele-Search" @click="handleQuery" v-auth="'sysOplog:page'"> 查询 </el-button>
					<el-button icon="ele-DeleteFilled" type="danger" @click="clearLog" v-auth="'sysOplog:clear'"> 清空 </el-button>
					<el-button icon="ele-FolderOpened" @click="exportLog" v-auth="'sysOplog:export'"> 导出 </el-button>
				</el-form-item>
			</el-form>
		</el-card>

		<el-card shadow="hover" style="margin-top: 8px">
			<el-table :data="logData" @sort-change="sortChange" style="width: 100%" border>
				<el-table-column type="index" label="序号" width="55" align="center" />
				<el-table-column prop="logName" label="类别名称" sortable="custom" show-overflow-tooltip />
				<el-table-column prop="logLevel" label="日志级别" width="110" align="center" show-overflow-tooltip />
				<el-table-column prop="eventId" label="事件Id" width="70" align="center" show-overflow-tooltip />
				<el-table-column prop="message" label="日志消息" show-overflow-tooltip />
				<el-table-column prop="state" label="当前状态值" show-overflow-tooltip />
				<el-table-column prop="threadId" label="线程Id" sortable="custom" width="90" align="center" show-overflow-tooltip />
				<el-table-column prop="traceId" label="请求跟踪Id" sortable="custom" show-overflow-tooltip />
				<el-table-column prop="logDateTime" label="记录时间" sortable="custom" align="center" show-overflow-tooltip />
				<el-table-column prop="exception" label="异常对象" show-overflow-tooltip />
				<el-table-column label="操作" width="80" align="center" fixed="right" show-overflow-tooltip>
					<template #default="scope">
						<el-button icon="ele-InfoFilled" size="small" text type="primary" @click="viewDetail(scope.row)" v-auth="'sysOplog:page'">详情 </el-button>
					</template>
				</el-table-column>
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
		<el-dialog v-model="dialogVisible" draggable width="769px">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Document /> </el-icon>
					<span> 日志详情 </span>
				</div>
			</template>
			<pre>{{ content }}</pre>
		</el-dialog>
	</div>
</template>

<script lang="ts">
import { toRefs, reactive, onMounted, defineComponent } from 'vue';
import { ElMessage } from 'element-plus';
import { downloadByData, getFileName } from '/@/utils/download';

import { getAPI } from '/@/utils/axios-utils';
import { SysLogOpApi } from '/@/api-services/api';
import { SysLogOp } from '/@/api-services/models';

export default defineComponent({
	name: 'sysOpLog',
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
				field: 'createTime', //默认的排序字段
				order: 'descending', //排序方向
				descStr: 'descending', //降序排序的关键字符，element-plus是descending； ant-design是descend
				total: 0 as any,
			},
			logData: [] as Array<SysLogOp>,
			dialogVisible: false,
			content: '',
		});
		onMounted(async () => {
			handleQuery();
		});
		// 查询操作
		const handleQuery = async () => {
			if (state.queryParams.startTime == null) state.queryParams.startTime = undefined;
			if (state.queryParams.endTime == null) state.queryParams.endTime = undefined;
			state.loading = true;
			var res = await getAPI(SysLogOpApi).apiSysLogOpGetLogOpPageGet(
				state.queryParams.startTime,
				state.queryParams.endTime,
				state.tableParams.page,
				state.tableParams.pageSize,
				state.tableParams.field,
				state.tableParams.order,
				state.tableParams.descStr
			);
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
			await getAPI(SysLogOpApi).apiSysLogOpClearLogOpPost();
			state.loading = false;

			ElMessage.success('清空成功');
			handleQuery();
		};
		// 导出日志
		const exportLog = async () => {
			state.loading = true;
			var res = await getAPI(SysLogOpApi).apiSysLogOpExporLogOpPost(state.queryParams, { responseType: 'blob' });
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
		// 查看详情
		const viewDetail = (row: any) => {
			state.content = row.message;
			state.dialogVisible = true;
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
		// 列排序
		const sortChange = (column: any) => {
			state.tableParams.field = column.prop;
			state.tableParams.order = column.order;
			handleQuery();
		};
		return {
			handleQuery,
			resetQuery,
			clearLog,
			exportLog,
			shortcuts,
			handleSizeChange,
			handleCurrentChange,
			viewDetail,
			sortChange,
			...toRefs(state),
		};
	},
});
</script>

<style lang="scss">
.el-popper {
	max-width: 60%;
}

pre {
	white-space: break-spaces;
	line-height: 20px;
}
</style>
