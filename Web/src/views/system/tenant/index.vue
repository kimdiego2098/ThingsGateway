<template>
	<div class="sys-tenant-container">
		<el-card shadow="hover" :body-style="{ paddingBottom: '0' }">
			<el-form :model="queryParams" ref="queryForm" :inline="true">
				<el-form-item label="租户名称" prop="name">
					<el-input placeholder="租户名称" clearable @keyup.enter="handleQuery" v-model="queryParams.name" />
				</el-form-item>
				<el-form-item label="联系电话" prop="phone">
					<el-input placeholder="联系电话" clearable @keyup.enter="handleQuery" v-model="queryParams.phone" />
				</el-form-item>
				<el-form-item>
					<el-button icon="ele-Refresh" @click="resetQuery"> 重置 </el-button>
					<el-button type="primary" icon="ele-Search" @click="handleQuery" v-auth="'sysTenant:page'"> 查询 </el-button>
					<el-button icon="ele-Plus" @click="openAddTenant" v-auth="'sysTenant:add'"> 新增 </el-button>
				</el-form-item>
			</el-form>
		</el-card>

		<el-card shadow="hover" style="margin-top: 8px">
			<el-table :data="tenantData" style="width: 100%" v-loading="loading" border>
				<el-table-column type="index" label="序号" width="55" align="center" fixed />
				<el-table-column prop="name" label="租户名称" width="160" show-overflow-tooltip />
				<el-table-column prop="adminName" label="管理员" width="120" show-overflow-tooltip />
				<el-table-column prop="phone" label="电话" width="120" show-overflow-tooltip />
				<!-- <el-table-column prop="host" label="主机" show-overflow-tooltip /> -->
				<!-- <el-table-column prop="email" label="邮箱" show-overflow-tooltip /> -->
				<el-table-column prop="tenantType" label="租户类型" width="100" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-tag v-if="scope.row.tenantType === 0"> ID隔离 </el-tag>
						<el-tag type="danger" v-else> 库隔离 </el-tag>
					</template>
				</el-table-column>
				<el-table-column label="状态" width="70" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-switch v-model="scope.row.status" :active-value="1" :inactive-value="2" size="small" @change="changeStatus(scope.row)" :disabled="scope.row.id == 123456780000000" />
					</template>
				</el-table-column>
				<el-table-column prop="dbType" label="数据库类型" width="120" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-tag v-if="scope.row.dbType === 0"> MySql </el-tag>
						<el-tag v-else-if="scope.row.dbType === 1"> SqlServer </el-tag>
						<el-tag v-if="scope.row.dbType === 2"> Sqlite </el-tag>
						<el-tag v-else-if="scope.row.dbType === 3"> Oracle </el-tag>
						<el-tag v-if="scope.row.dbType === 4"> PostgreSQL </el-tag>
						<el-tag v-else-if="scope.row.dbType === 5"> Dm </el-tag>
						<el-tag v-if="scope.row.dbType === 6"> Kdbndp </el-tag>
						<el-tag v-else-if="scope.row.dbType === 7"> Oscar </el-tag>
						<el-tag v-if="scope.row.dbType === 8"> MySqlConnector </el-tag>
						<el-tag v-else-if="scope.row.dbType === 9"> Access </el-tag>
						<el-tag v-if="scope.row.dbType === 10"> OpenGauss </el-tag>
						<el-tag v-else-if="scope.row.dbType === 11"> QuestDB </el-tag>
						<el-tag v-else-if="scope.row.dbType === 12"> HG </el-tag>
						<el-tag v-else-if="scope.row.dbType === 13"> ClickHouse </el-tag>
						<el-tag v-else-if="scope.row.dbType === 14"> GBase </el-tag>
						<el-tag v-else-if="scope.row.dbType === 900"> Custom </el-tag>
					</template>
				</el-table-column>
				<!-- <el-table-column prop="configId" label="数据库标识" show-overflow-tooltip /> -->
				<el-table-column prop="connection" label="数据库连接" width="300" show-overflow-tooltip />
				<el-table-column prop="orderNo" label="排序" width="70" align="center" show-overflow-tooltip />
				<el-table-column prop="createTime" label="修改时间" width="160" align="center" show-overflow-tooltip />
				<el-table-column prop="remark" label="备注" show-overflow-tooltip />
				<el-table-column label="操作" width="180" fixed="right" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-button icon="ele-Coin" size="small" text type="danger" @click="createTenant(scope.row)" v-auth="'sysTenant:createDb'" :disabled="scope.row.tenantType == 0"> 创建库 </el-button>
						<el-button icon="ele-Edit" size="small" text type="primary" @click="openEditTenant(scope.row)" v-auth="'sysTenant:update'"> 编辑 </el-button>
						<el-dropdown>
							<el-button icon="ele-MoreFilled" size="small" text type="primary" style="padding-left: 12px" />
							<template #dropdown>
								<el-dropdown-menu>
									<el-dropdown-item icon="ele-OfficeBuilding" @click="openGrantMenu(scope.row)" :v-auth="'sysTenant:grantMenu'"> 授权菜单 </el-dropdown-item>
									<el-dropdown-item icon="ele-RefreshLeft" @click="resetTenantPwd(scope.row)" :v-auth="'sysTenant:resetPwd'"> 重置密码 </el-dropdown-item>
									<el-dropdown-item icon="ele-Delete" @click="delTenant(scope.row)" :v-auth="'sysTenant:delete'"> 删除租户 </el-dropdown-item>
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
		<EditTenant ref="editTenantRef" :title="editTenantTitle" />
		<GrantMenu ref="grantMenuRef" />
	</div>
</template>

<script lang="ts">
import { ref, toRefs, reactive, onMounted, defineComponent, onUnmounted } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import mittBus from '/@/utils/mitt';
import EditTenant from '/@/views/system/tenant/component/editTenant.vue';
import GrantMenu from '/@/views/system/tenant/component/grantMenu.vue';

import { getAPI } from '/@/utils/axios-utils';
import { SysTenantApi } from '/@/api-services/api';
import { TenantOutput } from '/@/api-services/models';

export default defineComponent({
	name: 'sysTenant',
	components: { EditTenant, GrantMenu },
	setup() {
		const editTenantRef = ref();
		const grantMenuRef = ref();
		const state = reactive({
			loading: false,
			tenantData: [] as Array<TenantOutput>,
			queryParams: {
				name: undefined,
				phone: undefined,
			},
			tableParams: {
				page: 1,
				pageSize: 10,
				total: 0 as any,
			},
			editTenantTitle: '',
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
			var res = await getAPI(SysTenantApi).apiSysTenantGetTenantPageGet(state.queryParams.name, state.queryParams.phone, state.tableParams.page, state.tableParams.pageSize);
			state.tenantData = res.data.result?.items ?? [];
			state.tableParams.total = res.data.result?.total;
			state.loading = false;
		};
		// 重置操作
		const resetQuery = () => {
			state.queryParams.name = undefined;
			state.queryParams.phone = undefined;
			handleQuery();
		};
		// 打开新增页面
		const openAddTenant = () => {
			state.editTenantTitle = '添加租户';
			editTenantRef.value.openDialog({ tenantType: 0 });
		};
		// 打开编辑页面
		const openEditTenant = (row: any) => {
			state.editTenantTitle = '编辑租户';
			editTenantRef.value.openDialog(row);
		};
		// 打开授权菜单页面
		const openGrantMenu = async (row: any) => {
			grantMenuRef.value.openDialog(row);
		};
		// 重置密码
		const resetTenantPwd = async (row: any) => {
			ElMessageBox.confirm(`确定重置密码：【${row.name}】?`, '提示', {
				confirmButtonText: '确定',
				cancelButtonText: '取消',
				type: 'warning',
			})
				.then(async () => {
					await getAPI(SysTenantApi).apiSysTenantResetTenantPwdPost({ userId: row.userId });
					ElMessage.success('密码重置成功：123456');
				})
				.catch(() => {});
		};
		// 删除
		const delTenant = (row: any) => {
			ElMessageBox.confirm(`确定删除租户：【${row.name}】?`, '提示', {
				confirmButtonText: '确定',
				cancelButtonText: '取消',
				type: 'warning',
			})
				.then(async () => {
					await getAPI(SysTenantApi).apiSysTenantDeleteTenantPost({ id: row.id });
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
		// 创建租户库
		const createTenant = (row: any) => {
			ElMessageBox.confirm(`确定创建/更新租户数据库：【${row.name}】?`, '提示', {
				confirmButtonText: '确定',
				cancelButtonText: '取消',
				type: 'warning',
			})
				.then(async () => {
					await getAPI(SysTenantApi).apiSysTenantCreateTenantDbPost({ id: row.id });
					ElMessage.success('创建/更新租户数据库成功');
				})
				.catch(() => {});
		};
		// 修改状态
		const changeStatus = (row: any) => {
			getAPI(SysTenantApi)
				.apiSysTenantSetTenantStatusPost({ id: row.id, status: row.status })
				.then(() => {
					ElMessage.success('租户状态设置成功');
				})
				.catch(() => {
					row.status = row.status == 1 ? 2 : 1;
				});
		};
		return {
			editTenantRef,
			grantMenuRef,
			handleQuery,
			resetQuery,
			openAddTenant,
			openEditTenant,
			openGrantMenu,
			resetTenantPwd,
			delTenant,
			handleSizeChange,
			handleCurrentChange,
			createTenant,
			changeStatus,
			...toRefs(state),
		};
	},
});
</script>
