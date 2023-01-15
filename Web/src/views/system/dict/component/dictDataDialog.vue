<template>
	<div class="sys-dictData-container">
		<el-dialog v-model="isShowDialog" draggable width="860px">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Edit /> </el-icon>
					<span> 字典值列表 </span>
				</div>
			</template>
			<el-card shadow="hover" :body-style="{ paddingBottom: '0' }">
				<el-form :model="queryParams" ref="queryForm" :inline="true">
					<el-form-item label="值" prop="value">
						<el-input placeholder="值" clearable @keyup.enter="handleQuery" v-model="queryParams.value" />
					</el-form-item>
					<el-form-item label="编码" prop="code">
						<el-input placeholder="编码" clearable @keyup.enter="handleQuery" v-model="queryParams.code" />
					</el-form-item>
					<el-form-item>
						<el-button icon="ele-Refresh" @click="resetQuery"> 重置 </el-button>
						<el-button type="primary" icon="ele-Search" @click="handleQuery"> 查询 </el-button>
						<el-button icon="ele-Plus" @click="openAddDictData"> 新增 </el-button>
					</el-form-item>
				</el-form>
			</el-card>

			<el-card shadow="hover" style="margin-top: 8px">
				<el-table :data="dictDataData" style="width: 100%" v-loading="loading" border>
					<el-table-column type="index" label="序号" width="55" align="center" />
					<el-table-column prop="value" label="字典值" show-overflow-tooltip />
					<el-table-column prop="code" label="编码" show-overflow-tooltip />
					<el-table-column prop="status" label="状态" width="70" align="center" show-overflow-tooltip>
						<template #default="scope">
							<el-tag type="success" v-if="scope.row.status === 1">启用</el-tag>
							<el-tag type="danger" v-else>禁用</el-tag>
						</template>
					</el-table-column>
					<el-table-column prop="orderNo" label="排序" width="70" align="center" show-overflow-tooltip />
					<el-table-column prop="createTime" label="修改时间" align="center" show-overflow-tooltip />
					<el-table-column prop="remark" label="备注" show-overflow-tooltip />
					<el-table-column label="操作" width="140" fixed="right" align="center" show-overflow-tooltip>
						<template #default="scope">
							<el-button icon="ele-Edit" size="small" text type="primary" @click="openEditDictData(scope.row)"> 编辑 </el-button>
							<el-button icon="ele-Delete" size="small" text type="danger" @click="delDictData(scope.row)"> 删除 </el-button>
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
		</el-dialog>
		<EditDictData ref="editDictDataRef" :title="editDictDataTitle" :dictTypeId="dictTypeId" />
	</div>
</template>

<script lang="ts">
import { toRefs, reactive, onMounted, ref, defineComponent, onUnmounted } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import mittBus from '/@/utils/mitt';
import EditDictData from '/@/views/system/dict/component/editDictData.vue';

import { getAPI } from '/@/utils/axios-utils';
import { SysDictDataApi } from '/@/api-services/api';
import { SysDictData } from '/@/api-services/models';

export default defineComponent({
	name: 'sysDictData',
	components: { EditDictData },
	setup() {
		const editDictDataRef = ref();
		const state = reactive({
			isShowDialog: false,
			loading: false,
			dictDataData: [] as Array<SysDictData>,
			queryParams: {
				value: undefined,
				code: undefined,
			},
			tableParams: {
				page: 1,
				pageSize: 10,
				total: 0 as any,
			},
			editDictDataTitle: '',
			dictTypeId: 0, // 字典类型Id
		});
		onMounted(async () => {
			mittBus.on('submitRefreshDictData', () => {
				handleQuery();
			});
		});
		onUnmounted(() => {
			mittBus.off('submitRefreshDictData');
		});
		// 打开弹窗
		const openDialog = async (row: any) => {
			state.dictTypeId = row.id;
			handleQuery();
			state.isShowDialog = true;
		};
		// 查询操作
		const handleQuery = async () => {
			state.loading = true;
			var res = await getAPI(SysDictDataApi).apiSysDictDataGetDictDataPageGet(state.dictTypeId, state.queryParams.value, state.queryParams.code, state.tableParams.page, state.tableParams.pageSize);
			state.dictDataData = res.data.result?.items ?? [];
			state.tableParams.total = res.data.result?.total;
			state.loading = false;
		};
		// 重置操作
		const resetQuery = () => {
			state.queryParams.value = undefined;
			state.queryParams.code = undefined;
			handleQuery();
		};
		// 打开新增页面
		const openAddDictData = () => {
			state.editDictDataTitle = '添加字典值';
			editDictDataRef.value.openDialog({});
		};
		// 打开编辑页面
		const openEditDictData = (row: any) => {
			state.editDictDataTitle = '编辑字典值';
			editDictDataRef.value.openDialog(row);
		};
		// 删除
		const delDictData = (row: any) => {
			ElMessageBox.confirm(`确定删除字典值：【${row.name}】?`, '提示', {
				confirmButtonText: '确定',
				cancelButtonText: '取消',
				type: 'warning',
			})
				.then(async () => {
					await getAPI(SysDictDataApi).apiSysDictDataDeleteDictDataPost({ id: row.id });
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
			editDictDataRef,
			openDialog,
			handleQuery,
			resetQuery,
			openAddDictData,
			openEditDictData,
			delDictData,
			handleSizeChange,
			handleCurrentChange,
			...toRefs(state),
		};
	},
});
</script>
