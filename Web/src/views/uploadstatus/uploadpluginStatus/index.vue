<template>
	<div class="sys-uploadPlugin-container">
		<el-card shadow="hover" :body-style="{ paddingBottom: '0' }">
			<el-form :model="queryParams" ref="queryForm" :inline="true">
				<el-form-item label="名称" prop="uploadname">
					<el-input v-model="queryParams.uploadname" clearable placeholder="名称" @keyup.enter="handleQuery" />
				</el-form-item>
				<el-form-item>
					<el-button icon="ele-Refresh" @click="resetQuery"> 重置 </el-button>
					<el-button type="primary" icon="ele-Search" @click="handleQuery" v-auth="'uploadPlugin:page'"> 查询
					</el-button>
				</el-form-item>
			</el-form>
		</el-card>

		<el-card shadow="hover" style="margin-top: 8px">
			<el-table :data="uploadPlugin" style="width: 100%" v-loading="loading" border>
				<el-table-column type="expand" fixed>
					<template #default="scope">
						<el-table :data="scope.row.pluginAssemble" border size="small">
							<el-table-column prop="assembleName" label="子驱动名称" width="150" show-overflow-tooltip />
							<el-table-column prop="type" label="子驱动程序集信息" show-overflow-tooltip />
						</el-table>
					</template>
				</el-table-column>

				<el-table-column type="index" label="序号" width="55" align="center" />
				<el-table-column prop="pluginName" label="插件名称" show-overflow-tooltip />
				<el-table-column prop="fileName" label="插件路径" show-overflow-tooltip />

			</el-table>
			<el-pagination v-model:currentPage="tableParams.page" v-model:page-size="tableParams.pageSize"
				:total="tableParams.total" :page-sizes="[10, 20, 50, 100]" small background
				@size-change="handleSizeChange" @current-change="handleCurrentChange"
				layout="total, sizes, prev, pager, next, jumper" />
		</el-card>
	</div>
</template>

<script lang="ts">
import { toRefs, reactive, onMounted, ref, defineComponent, onUnmounted } from 'vue';
import mittBus from '/@/utils/mitt';

import { getAPI } from '/@/utils/axios-utils';
import { UploadPluginRunTimeApi } from '/@/api-services/api';
import { PluginInfo } from '/@/api-services/models';

export default defineComponent({
	name: 'uploadPlugin',
	components: {},
	setup() {
		const editUploadPluginRef = ref();
		const state = reactive({
			loading: false,
			queryParams: {
				uploadname: undefined,
			},
			tableParams: {
				page: 1,
				pageSize: 10,
				total: 0 as any,
			},
			uploadPlugin: [] as Array<PluginInfo>,
			edituploadPluginTitle: '',

		});
		onMounted(async () => {
			handleQuery();

			mittBus.on('devicePluginRefresh', () => {
				handleQuery();
			});
		});
		onUnmounted(() => {
			mittBus.off('devicePluginRefresh');
		});
		// 查询操作
		const handleQuery = async () => {
			if (state.queryParams.uploadname == null) state.queryParams.uploadname = undefined;
			state.loading = true;
			var res = await getAPI(UploadPluginRunTimeApi).apiUploadPluginRunTimeGetUploadPluginPageGet(state.queryParams.uploadname, undefined, state.tableParams.page, state.tableParams.pageSize);
			state.uploadPlugin = res.data.result?.items ?? [];
			state.tableParams.total = res.data.result?.total;
			state.loading = false;
		};
		// 重置操作
		const resetQuery = () => {
			state.queryParams.uploadname = undefined;
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
			editUploadPluginRef,
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
