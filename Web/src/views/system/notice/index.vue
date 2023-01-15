<template>
	<div class="sys-notice-container">
		<el-card shadow="hover" :body-style="{ paddingBottom: '0' }">
			<el-form :model="queryParams" ref="queryForm" :inline="true">
				<el-form-item label="标题" prop="title">
					<el-input placeholder="标题" clearable @keyup.enter="handleQuery" v-model="queryParams.title" />
				</el-form-item>
				<el-form-item label="类型" prop="type">
					<el-select v-model="queryParams.type" placeholder="类型" style="width: 100%">
						<el-option label="通知" :value="1" />
						<el-option label="公告" :value="2" />
					</el-select>
				</el-form-item>
				<el-form-item>
					<el-button icon="ele-Refresh" @click="resetQuery"> 重置 </el-button>
					<el-button type="primary" icon="ele-Search" @click="handleQuery" v-auth="'sysNotice:page'"> 查询 </el-button>
					<el-button icon="ele-Plus" @click="openAddNotice" v-auth="'sysNotice:add'"> 新增 </el-button>
				</el-form-item>
			</el-form>
		</el-card>

		<el-card shadow="hover" style="margin-top: 8px">
			<el-table :data="noticeData" style="width: 100%" v-loading="loading" border>
				<el-table-column type="index" label="序号" width="55" align="center" />
				<el-table-column prop="title" label="标题" show-overflow-tooltip />
				<el-table-column prop="content" label="内容" show-overflow-tooltip>
					<template #default="scope"> {{ removeHtml(scope.row.content) }} </template>
				</el-table-column>
				<el-table-column prop="type" label="类型" width="100" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-tag v-if="scope.row.type === 1"> 通知 </el-tag>
						<el-tag type="warning" v-else> 公告 </el-tag>
					</template>
				</el-table-column>
				<el-table-column prop="createTime" label="创建时间" align="center" show-overflow-tooltip />
				<el-table-column prop="status" label="状态" width="100" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-tag type="info" v-if="scope.row.status === 1"> 已发布 </el-tag>
						<el-tag type="warning" v-else> 未发布 </el-tag>
					</template>
				</el-table-column>
				<el-table-column prop="publicUserName" label="发布者" align="center" show-overflow-tooltip />
				<el-table-column prop="publicTime" label="发布时间" align="center" show-overflow-tooltip />
				<el-table-column label="操作" width="200" fixed="right" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-button icon="ele-Position" size="small" text type="primary" @click="publicNotice(scope.row)" v-auth="'sysNotice:public'" :disabled="scope.row.status === 1"> 发布 </el-button>
						<el-button icon="ele-Edit" size="small" text type="primary" @click="openEditNotice(scope.row)" v-auth="'sysNotice:update'" :disabled="scope.row.status === 1"> 编辑 </el-button>
						<el-button icon="ele-Delete" size="small" text type="danger" @click="delNotice(scope.row)" v-auth="'sysNotice:delete'" :disabled="scope.row.status === 1"> 删除 </el-button>
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
		<EditNotice ref="editNoticeRef" :title="editNoticeTitle" />
	</div>
</template>

<script lang="ts">
import { toRefs, reactive, onMounted, ref, defineComponent, onUnmounted } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import commonFunction from '/@/utils/commonFunction';
import mittBus from '/@/utils/mitt';
import EditNotice from '/@/views/system/notice/component/editNotice.vue';

import { getAPI } from '/@/utils/axios-utils';
import { SysNoticeApi } from '/@/api-services/api';
import { SysNotice } from '/@/api-services/models';

export default defineComponent({
	name: 'sysNotice',
	components: { EditNotice },
	setup() {
		const editNoticeRef = ref();
		const { removeHtml } = commonFunction();
		const state = reactive({
			loading: false,
			noticeData: [] as Array<SysNotice>,
			queryParams: {
				title: undefined,
				type: undefined,
			},
			tableParams: {
				page: 1,
				pageSize: 10,
				total: 0 as any,
			},
			editNoticeTitle: '',
		});
		onMounted(async () => {
			handleQuery();

			mittBus.on('submitRefresh', () => {
				handleQuery();
			});
		});
		onUnmounted(() => {
			mittBus.off('submitRefresh');
		});
		// 查询操作
		const handleQuery = async () => {
			state.loading = true;
			var res = await getAPI(SysNoticeApi).apiSysNoticeGetPageNoticeGet(state.queryParams.title, state.queryParams.type, state.tableParams.page, state.tableParams.pageSize);
			state.noticeData = res.data.result?.items ?? [];
			state.tableParams.total = res.data.result?.total;
			state.loading = false;
		};
		// 重置操作
		const resetQuery = () => {
			state.queryParams.title = undefined;
			state.queryParams.type = undefined;
			handleQuery();
		};
		// 打开新增页面
		const openAddNotice = () => {
			state.editNoticeTitle = '添加通知公告';
			editNoticeRef.value.openDialog({});
		};
		// 打开编辑页面
		const openEditNotice = (row: any) => {
			state.editNoticeTitle = '编辑通知公告';
			editNoticeRef.value.openDialog(row);
		};
		// 删除
		const delNotice = (row: any) => {
			ElMessageBox.confirm(`确定删除通知公告：【${row.title}】?`, '提示', {
				confirmButtonText: '确定',
				cancelButtonText: '取消',
				type: 'warning',
			})
				.then(async () => {
					await getAPI(SysNoticeApi).apiSysNoticeDeleteNoticePost({ id: row.id });
					handleQuery();
					ElMessage.success('删除成功');
				})
				.catch(() => {});
		};
		// 发布
		const publicNotice = (row: any) => {
			ElMessageBox.confirm(`确定发布通知公告：【${row.title}】，不可撤销?`, '提示', {
				confirmButtonText: '确定',
				cancelButtonText: '取消',
				type: 'warning',
			})
				.then(async () => {
					await getAPI(SysNoticeApi).apiSysNoticePublicNoticePost({ id: row.id });
					handleQuery();
					ElMessage.success('发布成功');
				})
				.catch(() => {});
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
			editNoticeRef,
			handleQuery,
			resetQuery,
			openAddNotice,
			openEditNotice,
			delNotice,
			publicNotice,
			handleSizeChange,
			handleCurrentChange,
			removeHtml,
			...toRefs(state),
		};
	},
});
</script>
