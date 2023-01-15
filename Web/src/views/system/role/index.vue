<template>
	<div class="sys-role-container">
		<el-card shadow="hover" :body-style="{ paddingBottom: '0' }">
			<el-form :model="queryParams" ref="queryForm" :inline="true">
				<el-form-item label="角色名称" prop="name">
					<el-input placeholder="角色名称" clearable @keyup.enter="handleQuery" v-model="queryParams.name" />
				</el-form-item>
				<el-form-item label="角色编码" prop="code">
					<el-input placeholder="角色编码" clearable @keyup.enter="handleQuery" v-model="queryParams.code" />
				</el-form-item>
				<el-form-item>
					<el-button icon="ele-Refresh" @click="resetQuery"> 重置 </el-button>
					<el-button type="primary" icon="ele-Search" @click="handleQuery" v-auth="'sysRole:page'"> 查询 </el-button>
					<el-button icon="ele-Plus" @click="openAddRole" v-auth="'sysRole:add'"> 新增 </el-button>
				</el-form-item>
			</el-form>
		</el-card>

		<el-card shadow="hover" style="margin-top: 8px">
			<el-table :data="roleData" style="width: 100%" v-loading="loading" border>
				<el-table-column type="index" label="序号" width="55" align="center" fixed />
				<el-table-column prop="name" label="角色名称" show-overflow-tooltip />
				<el-table-column prop="code" label="角色编码" show-overflow-tooltip />
				<el-table-column label="数据范围" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-tag effect="plain" v-if="scope.row.dataScope === 1">全部数据</el-tag>
						<el-tag effect="plain" v-else-if="scope.row.dataScope === 2">本部门及以下数据</el-tag>
						<el-tag effect="plain" v-else-if="scope.row.dataScope === 3">本部门数据</el-tag>
						<el-tag effect="plain" v-else-if="scope.row.dataScope === 4">仅本人数据</el-tag>
						<el-tag effect="plain" v-else-if="scope.row.dataScope === 5">自定义数据</el-tag>
					</template>
				</el-table-column>
				<el-table-column prop="orderNo" label="排序" width="70" align="center" show-overflow-tooltip />
				<el-table-column label="状态" width="70" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-tag type="success" v-if="scope.row.status === 1">启用</el-tag>
						<el-tag type="danger" v-else>禁用</el-tag>
					</template>
				</el-table-column>
				<el-table-column prop="createTime" label="修改时间" align="center" show-overflow-tooltip />
				<el-table-column prop="remark" label="备注" show-overflow-tooltip />
				<el-table-column label="操作" width="110" fixed="right" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-button icon="ele-Edit" size="small" text type="primary" @click="openEditRole(scope.row)" v-auth="'sysRole:update'"> 编辑 </el-button>
						<el-dropdown>
							<el-button icon="ele-MoreFilled" size="small" text type="primary" style="padding-left: 12px" />
							<template #dropdown>
								<el-dropdown-menu>
									<el-dropdown-item icon="ele-OfficeBuilding" @click="openGrantData(scope.row)" :disabled="!auth('sysRole:grantData')"> 数据范围 </el-dropdown-item>
									<el-dropdown-item icon="ele-Delete" @click="delRole(scope.row)" divided :disabled="!auth('sysRole:delete')"> 删除角色 </el-dropdown-item>
								</el-dropdown-menu>
							</template>
						</el-dropdown>
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
		<EditRole ref="editRoleRef" :title="editRoleTitle" />
		<GrantData ref="grantDataRef" />
	</div>
</template>

<script lang="ts">
import { ref, toRefs, reactive, onMounted, defineComponent, onUnmounted } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import mittBus from '/@/utils/mitt';
import { auth } from '/@/utils/authFunction';
import EditRole from '/@/views/system/role/component/editRole.vue';
import GrantData from '/@/views/system/role/component/grantData.vue';

import { getAPI } from '/@/utils/axios-utils';
import { SysRoleApi } from '/@/api-services/api';
import { SysRole } from '/@/api-services/models';

export default defineComponent({
	name: 'sysRole',
	components: { EditRole, GrantData },
	setup() {
		const editRoleRef = ref();
		const grantDataRef = ref();
		const state = reactive({
			loading: false,
			roleData: [] as Array<SysRole>,
			queryParams: {
				name: undefined,
				code: undefined,
			},
			tableParams: {
				page: 1,
				pageSize: 10,
				total: 0 as any,
			},
			editRoleTitle: '',
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
			var res = await getAPI(SysRoleApi).apiSysRoleGetRolePageGet(state.queryParams.name, state.queryParams.code, state.tableParams.page, state.tableParams.pageSize);
			state.roleData = res.data.result?.items ?? [];
			state.tableParams.total = res.data.result?.total;
			state.loading = false;
		};
		// 重置操作
		const resetQuery = () => {
			state.queryParams.name = undefined;
			state.queryParams.code = undefined;
			handleQuery();
		};
		// 打开新增页面
		const openAddRole = () => {
			state.editRoleTitle = '添加角色';
			editRoleRef.value.openDialog({});
		};
		// 打开编辑页面
		const openEditRole = async (row: any) => {
			state.editRoleTitle = '编辑角色';
			editRoleRef.value.openDialog(row);
		};
		// 打开授权数据范围页面
		const openGrantData = (row: any) => {
			grantDataRef.value.openDialog(row);
		};
		// 删除
		const delRole = (row: any) => {
			ElMessageBox.confirm(`确定删角色：【${row.name}】?`, '提示', {
				confirmButtonText: '确定',
				cancelButtonText: '取消',
				type: 'warning',
			})
				.then(async () => {
					await getAPI(SysRoleApi).apiSysRoleDeleteRolePost({ id: row.id });
					handleQuery();
					ElMessage.success('删除成功');
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
			editRoleRef,
			grantDataRef,
			handleQuery,
			resetQuery,
			openAddRole,
			openEditRole,
			openGrantData,
			delRole,
			handleSizeChange,
			handleCurrentChange,
			auth,
			...toRefs(state),
		};
	},
});
</script>
