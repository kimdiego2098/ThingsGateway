<template>
	<div class="sys-menu-container">
		<el-card shadow="hover" :body-style="{ paddingBottom: '0' }">
			<el-form :model="queryParams" ref="queryForm" :inline="true">
				<el-form-item label="菜单名称" prop="title">
					<el-input placeholder="菜单名称" clearable @keyup.enter="handleQuery" v-model="queryParams.title" />
				</el-form-item>
				<el-form-item label="类型" prop="type">
					<el-select v-model="queryParams.type" placeholder="类型" clearable>
						<el-option label="目录" :value="1" />
						<el-option label="菜单" :value="2" />
						<el-option label="按钮" :value="3" />
					</el-select>
				</el-form-item>
				<el-form-item>
					<el-button icon="ele-Refresh" @click="resetQuery"> 重置 </el-button>
					<el-button type="primary" icon="ele-Search" @click="handleQuery" v-auth="'sysMenu:list'"> 查询 </el-button>
					<el-button icon="ele-Plus" @click="openAddMenu" v-auth="'sysMenu:add'"> 新增 </el-button>
				</el-form-item>
			</el-form>
		</el-card>

		<el-card shadow="hover" style="margin-top: 8px">
			<el-table :data="menuData" v-loading="loading" row-key="id" :tree-props="{ children: 'children', hasChildren: 'hasChildren' }" border>
				<el-table-column label="菜单名称" show-overflow-tooltip>
					<template #default="scope">
						<SvgIcon :name="scope.row.icon" />
						<span class="ml10">{{ $t(scope.row.title) }}</span>
					</template>
				</el-table-column>
				<el-table-column label="类型" width="70" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-tag type="warning" v-if="scope.row.type === 1">目录</el-tag>
						<el-tag v-else-if="scope.row.type === 2">菜单</el-tag>
						<el-tag type="info" v-else>按钮</el-tag>
					</template>
				</el-table-column>
				<el-table-column prop="path" label="路由路径" show-overflow-tooltip />
				<el-table-column prop="component" label="组件路径" show-overflow-tooltip />
				<el-table-column prop="permission" label="权限标识" show-overflow-tooltip />
				<el-table-column prop="orderNo" label="排序" width="70" align="center" show-overflow-tooltip />
				<el-table-column label="状态" width="80" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-tag type="success" v-if="scope.row.status === 1">启用</el-tag>
						<el-tag type="danger" v-else>禁用</el-tag>
					</template>
				</el-table-column>
				<el-table-column prop="createTime" label="修改时间" align="center" show-overflow-tooltip />
				<el-table-column label="操作" width="140" fixed="right" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-button icon="ele-Edit" size="small" text type="primary" @click="openEditMenu(scope.row)" v-auth="'sysMenu:update'"> 编辑 </el-button>
						<el-button icon="ele-Delete" size="small" text type="danger" @click="delMenu(scope.row)" v-auth="'sysMenu:delete'"> 删除 </el-button>
					</template>
				</el-table-column>
			</el-table>
		</el-card>
		<EditMenu ref="editMenuRef" :title="editMenuTitle" :menuData="menuData" />
	</div>
</template>

<script lang="ts">
import { ref, toRefs, reactive, defineComponent, onMounted, onUnmounted } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import mittBus from '/@/utils/mitt';
import EditMenu from '/@/views/system/menu/component/editMenu.vue';

import { getAPI } from '/@/utils/axios-utils';
import { SysMenuApi } from '/@/api-services/api';
import { SysMenu } from '/@/api-services/models';

export default defineComponent({
	name: 'sysMenu',
	components: { EditMenu },
	setup() {
		const editMenuRef = ref();
		const state = reactive({
			loading: false,
			menuData: [] as Array<SysMenu>,
			queryParams: {
				title: undefined,
				type: undefined,
			},
			editMenuTitle: '',
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
			var res = await getAPI(SysMenuApi).apiSysMenuGetMenuListGet(state.queryParams.title, state.queryParams.type);
			state.menuData = res.data.result ?? [];
			state.loading = false;
		};
		// 重置操作
		const resetQuery = () => {
			state.queryParams.title = undefined;
			state.queryParams.type = undefined;
			handleQuery();
		};
		// 打开新增页面
		const openAddMenu = () => {
			state.editMenuTitle = '添加菜单';
			editMenuRef.value.openDialog({ type: 2 });
		};
		// 打开编辑页面
		const openEditMenu = (row: any) => {
			state.editMenuTitle = '编辑菜单';
			editMenuRef.value.openDialog(row);
		};
		// 删除当前行
		const delMenu = (row: any) => {
			ElMessageBox.confirm(`确定删除菜单：【${row.title}】?`, '提示', {
				confirmButtonText: '确定',
				cancelButtonText: '取消',
				type: 'warning',
			})
				.then(async () => {
					await getAPI(SysMenuApi).apiSysMenuDeleteMenuPost({ id: row.id });
					handleQuery();
					ElMessage.success('删除成功');
				})
				.catch(() => {});
		};
		return {
			editMenuRef,
			handleQuery,
			resetQuery,
			openAddMenu,
			openEditMenu,
			delMenu,
			...toRefs(state),
		};
	},
});
</script>
