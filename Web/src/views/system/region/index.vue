<template>
	<div class="sys-region-container">
		<el-row :gutter="8" style="width: 100%">
			<el-col :span="6" :xs="24">
				<RegionTree ref="regionTreeRef" @node-click="nodeClick" />
			</el-col>

			<el-col :span="18" :xs="24">
				<el-card shadow="hover" :body-style="{ paddingBottom: '0' }">
					<el-form :model="queryParams" ref="queryForm" :inline="true">
						<el-form-item label="行政名称" prop="name">
							<el-input placeholder="行政名称" clearable @keyup.enter="handleQuery" v-model="queryParams.name" />
						</el-form-item>
						<el-form-item label="行政代码" prop="code">
							<el-input placeholder="行政代码" clearable @keyup.enter="handleQuery" v-model="queryParams.code" />
						</el-form-item>
						<el-form-item>
							<el-button icon="ele-Refresh" @click="resetQuery"> 重置 </el-button>
							<el-button type="primary" icon="ele-Search" @click="handleQuery" v-auth="'sysRegion:page'"> 查询 </el-button>
							<el-button icon="ele-Plus" @click="openAddRegion" v-auth="'sysRegion:add'"> 新增 </el-button>
							<el-button type="danger" icon="ele-Lightning" @click="handlSync" v-auth="'sysRegion:sync'"> 同步国家统计局 </el-button>
						</el-form-item>
					</el-form>
				</el-card>

				<el-card shadow="hover" style="margin-top: 8px">
					<el-table :data="regionData" style="width: 100%" v-loading="loading" row-key="id" default-expand-all :tree-props="{ children: 'children', hasChildren: 'hasChildren' }" border>
						<el-table-column prop="name" label="行政名称" show-overflow-tooltip />
						<el-table-column prop="code" label="行政代码" show-overflow-tooltip />
						<el-table-column prop="cityCode" label="区号" show-overflow-tooltip />
						<el-table-column prop="orderNo" label="排序" width="70" align="center" show-overflow-tooltip />
						<el-table-column prop="remark" label="备注" show-overflow-tooltip />
						<el-table-column label="操作" width="140" fixed="right" align="center" show-overflow-tooltip>
							<template #default="scope">
								<el-button icon="ele-Edit" size="small" text type="primary" @click="openEditRegion(scope.row)" v-auth="'sysRegion:update'"> 编辑 </el-button>
								<el-button icon="ele-Delete" size="small" text type="danger" @click="delRegion(scope.row)" v-auth="'sysRegion:delete'"> 删除 </el-button>
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
			</el-col>
		</el-row>
		<EditRegion ref="editRegionRef" :title="editRegionTitle" />
	</div>
</template>

<script lang="ts">
import { ref, toRefs, reactive, onMounted, defineComponent, onUnmounted } from 'vue';
import { ElMessageBox, ElMessage, ElNotification } from 'element-plus';
import mittBus from '/@/utils/mitt';
import RegionTree from '/@/views/system/region/component/regionTree.vue';
import EditRegion from '/@/views/system/region/component/editRegion.vue';

import { getAPI } from '/@/utils/axios-utils';
import { SysRegionApi } from '/@/api-services/api';
import { SysRegion } from '/@/api-services/models';

export default defineComponent({
	name: 'sysRegion',
	components: { RegionTree, EditRegion },
	setup() {
		const editRegionRef = ref();
		const regionTreeRef = ref();
		const state = reactive({
			loading: false,
			regionData: [] as Array<SysRegion>, // 列表数据
			queryParams: {
				id: -1,
				name: undefined,
				code: undefined,
			},
			tableParams: {
				page: 1,
				pageSize: 10,
				total: 0 as any,
			},
			editRegionTitle: '',
		});
		onMounted(() => {
			handleQuery();

			mittBus.on('submitRefresh', async () => {
				handleQuery();

				// 编辑删除后更新机构数据
				regionTreeRef.value.initTreeData();
			});
		});
		onUnmounted(() => {
			mittBus.off('submitRefresh');
		});
		// 查询操作
		const handleQuery = async () => {
			state.loading = true;
			var res = await getAPI(SysRegionApi).apiSysRegionGetRegionPageGet(state.queryParams.id, state.queryParams.name, state.queryParams.code, state.tableParams.page, state.tableParams.pageSize);
			state.regionData = res.data.result?.items ?? [];
			state.tableParams.total = res.data.result?.total;
			state.loading = false;
		};
		// 重置操作
		const resetQuery = () => {
			state.queryParams.id = -1;
			state.queryParams.name = undefined;
			state.queryParams.code = undefined;
			handleQuery();
		};
		// 打开新增页面
		const openAddRegion = () => {
			state.editRegionTitle = '添加行政区域';
			editRegionRef.value.openDialog({});
		};
		// 打开编辑页面
		const openEditRegion = (row: any) => {
			state.editRegionTitle = '编辑行政区域';
			editRegionRef.value.openDialog(row);
		};
		// 删除
		const delRegion = (row: any) => {
			ElMessageBox.confirm(`确定删除行政区域：【${row.name}】?`, '提示', {
				confirmButtonText: '确定',
				cancelButtonText: '取消',
				type: 'warning',
			})
				.then(async () => {
					await getAPI(SysRegionApi).apiSysRegionDeleteRegionPost({ id: row.id });
					ElMessage.success('删除成功');
					mittBus.emit('submitRefresh');
				})
				.catch(() => {});
		};
		// 树组件点击
		const nodeClick = async (node: any) => {
			state.queryParams.id = node.id;
			state.queryParams.name = undefined;
			state.queryParams.code = undefined;
			handleQuery();
		};
		// 同步国家统计局操作
		const handlSync = async () => {
			ElMessageBox.confirm('确认同步国家统计局行政区域数据？', '提示', {
				confirmButtonText: '确定',
				cancelButtonText: '取消',
				type: 'warning',
			})
				.then(async () => {
					ElNotification({
						title: '提示',
						message: '后台努力同步中...',
						type: 'success',
						position: 'bottom-right',
					});
					await getAPI(SysRegionApi).apiSysRegionSyncRegionPost({ timeout: 1000 * 60 * 30 });
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
			editRegionRef,
			regionTreeRef,
			handleQuery,
			resetQuery,
			openAddRegion,
			openEditRegion,
			delRegion,
			nodeClick,
			handlSync,
			handleSizeChange,
			handleCurrentChange,
			...toRefs(state),
		};
	},
});
</script>
