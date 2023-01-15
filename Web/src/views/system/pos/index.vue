<template>
	<div class="sys-pos-container">
		<el-card shadow="hover" :body-style="{ paddingBottom: '0' }">
			<el-form :model="queryParams" ref="queryForm" :inline="true">
				<el-form-item label="职位名称" prop="name">
					<el-input placeholder="职位名称" clearable @keyup.enter="handleQuery" v-model="queryParams.name" />
				</el-form-item>
				<el-form-item label="职位编码" prop="code">
					<el-input placeholder="职位编码" clearable @keyup.enter="handleQuery" v-model="queryParams.code" />
				</el-form-item>
				<el-form-item>
					<el-button icon="ele-Refresh" @click="resetQuery"> 重置 </el-button>
					<el-button type="primary" icon="ele-Search" @click="handleQuery" v-auth="'sysPos:list'"> 查询 </el-button>
					<el-button icon="ele-Plus" @click="openAddPos" v-auth="'sysPos:add'"> 新增 </el-button>
				</el-form-item>
			</el-form>
		</el-card>

		<el-card shadow="hover" style="margin-top: 8px">
			<el-table :data="posData" style="width: 100%" v-loading="loading" border>
				<el-table-column type="index" label="序号" width="55" align="center" />
				<el-table-column prop="name" label="职位名称" show-overflow-tooltip />
				<el-table-column prop="code" label="职位编码" show-overflow-tooltip />
				<el-table-column prop="orderNo" label="排序" width="70" align="center" show-overflow-tooltip />
				<el-table-column label="状态" width="70" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-tag type="success" v-if="scope.row.status === 1">启用</el-tag>
						<el-tag type="danger" v-else>禁用</el-tag>
					</template>
				</el-table-column>
				<el-table-column prop="createTime" label="修改时间" align="center" show-overflow-tooltip />
				<el-table-column prop="remark" label="备注" show-overflow-tooltip />
				<el-table-column label="操作" width="140" fixed="right" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-button icon="ele-Edit" size="small" text type="primary" @click="openEditPos(scope.row)" v-auth="'sysPos:update'"> 编辑 </el-button>
						<el-button icon="ele-Delete" size="small" text type="danger" @click="delPos(scope.row)" v-auth="'sysPos:delete'"> 删除 </el-button>
					</template>
				</el-table-column>
			</el-table>
		</el-card>
		<EditPos ref="editPosRef" :title="editPosTitle" />
	</div>
</template>

<script lang="ts">
import { toRefs, reactive, onMounted, ref, defineComponent, onUnmounted } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import mittBus from '/@/utils/mitt';
import EditPos from '/@/views/system/pos/component/editPos.vue';

import { getAPI } from '/@/utils/axios-utils';
import { SysPosApi } from '/@/api-services/api';
import { SysPos } from '/@/api-services/models';

export default defineComponent({
	name: 'sysPos',
	components: { EditPos },
	setup() {
		const editPosRef = ref();
		const state = reactive({
			loading: false,
			posData: [] as Array<SysPos>,
			queryParams: {
				name: undefined,
				code: undefined,
			},
			editPosTitle: '',
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
			var res = await getAPI(SysPosApi).apiSysPosGetPosListGet(state.queryParams.name, state.queryParams.code);
			state.posData = res.data.result ?? [];
			state.loading = false;
		};
		// 重置操作
		const resetQuery = () => {
			state.queryParams.name = undefined;
			state.queryParams.code = undefined;
			handleQuery();
		};
		// 打开新增页面
		const openAddPos = () => {
			state.editPosTitle = '添加职位';
			editPosRef.value.openDialog({ status: 1 });
		};
		// 打开编辑页面
		const openEditPos = (row: any) => {
			state.editPosTitle = '编辑职位';
			editPosRef.value.openDialog(row);
		};
		// 删除
		const delPos = (row: any) => {
			ElMessageBox.confirm(`确定删除职位：【${row.name}】?`, '提示', {
				confirmButtonText: '确定',
				cancelButtonText: '取消',
				type: 'warning',
			})
				.then(async () => {
					await getAPI(SysPosApi).apiSysPosDeletePosPost({ id: row.id });
					handleQuery();
					ElMessage.success('删除成功');
				})
				.catch(() => {});
		};
		return {
			editPosRef,
			handleQuery,
			resetQuery,
			openAddPos,
			openEditPos,
			delPos,
			...toRefs(state),
		};
	},
});
</script>
