<template>
	<div class="sys-driverPlugin-container">
		<el-card shadow="hover" :body-style="{ paddingBottom: '0' }">
			<el-form :model="queryParams" ref="queryForm" :inline="true">
				<el-form-item label="名称" prop="pluginName">
					<el-input v-model="queryParams.pluginName" clearable placeholder="名称" @keyup.enter="handleQuery" />
				</el-form-item>
				<el-form-item>
					<el-button icon="ele-Refresh" @click="resetQuery"> 重置 </el-button>
					<el-button type="primary" icon="ele-Search" @click="handleQuery" v-auth="'driverPlugin:page'"> 查询
					</el-button>
					<el-button icon="ele-Plus" @click="openAdd" v-auth="'driverPlugin:add'"> 新增 </el-button>
				</el-form-item>
			</el-form>
		</el-card>

		<el-card shadow="hover" style="margin-top: 8px">
			<el-table :data="driverPlugin" style="width: 100%" v-loading="loading" border>
				<el-table-column type="index" label="序号" width="55" align="center" />
				<el-table-column prop="pluginName" label="驱动名称" show-overflow-tooltip />
				<el-table-column prop="fileName" label="驱动路径" show-overflow-tooltip />

				<el-table-column label="操作" width="140" fixed="right" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-button icon="ele-Delete" size="small" text type="danger" @click="del(scope.row)"
							v-auth="'driverPlugin:delete'"> 删除 </el-button>
					</template>
				</el-table-column>

			</el-table>
			<el-pagination v-model:currentPage="tableParams.page" v-model:page-size="tableParams.pageSize"
				:total="tableParams.total" :page-sizes="[10, 20, 50, 100]" small background
				@size-change="handleSizeChange" @current-change="handleCurrentChange"
				layout="total, sizes, prev, pager, next, jumper" />
		</el-card>
		<EditDriverPlugin ref="editDriverPluginRef" :title="editdriverPluginTitle" />
	</div>
</template>

<script lang="ts">
import { toRefs, reactive, onMounted, ref, defineComponent, onUnmounted } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import mittBus from '/@/utils/mitt';

import EditDriverPlugin from './component/editDriverPlugin.vue';

import { getAPI } from '/@/utils/axios-utils';
import { DriverPluginApi } from '/@/api-services/api';
import { DriverPlugin } from '/@/api-services/models';

export default defineComponent({
	name: 'driverPlugin',
	components: { EditDriverPlugin },
	setup() {
		const editDriverPluginRef = ref();
		const state = reactive({
			loading: false,
			queryParams: {
				pluginName: undefined,
			},
			tableParams: {
				page: 1,
				pageSize: 10,
				total: 0 as any,
			},
			driverPlugin: [] as Array<DriverPlugin>,
			editdriverPluginTitle: '',

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
			if (state.queryParams.pluginName == null) state.queryParams.pluginName = undefined;
			state.loading = true;
			var res = await getAPI(DriverPluginApi).apiDriverPluginGetDriverPluginPageGet(state.queryParams.pluginName, undefined, state.tableParams.page, state.tableParams.pageSize);
			state.driverPlugin = res.data.result?.items ?? [];
			state.tableParams.total = res.data.result?.total;
			state.loading = false;
		};
		// 重置操作
		const resetQuery = () => {
			state.queryParams.pluginName = undefined;
			handleQuery();
		};
		// 打开新增页面
		const openAdd = () => {
			state.editdriverPluginTitle = '添加配置';
			editDriverPluginRef.value.openDialog({});
		};
		// 删除
		const del = (row: any) => {
			ElMessageBox.confirm(`确定删除驱动：【${row.name}】? 该操作不会删除驱动文件，后续可再次添加`, '提示', {
				confirmButtonText: '确定',
				cancelButtonText: '取消',
				type: 'warning',
			})
				.then(async () => {
					await getAPI(DriverPluginApi).apiDriverPluginDeleteConfigPost({ id: row.id });
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
			editDriverPluginRef,
			del,
			openAdd,
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
