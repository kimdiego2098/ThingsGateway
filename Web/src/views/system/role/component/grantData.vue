<template>
	<div class="sys-grantData-container">
		<el-dialog v-model="isShowDialog" draggable width="450px">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Edit /> </el-icon>
					<span> 授权数据范围 </span>
				</div>
			</template>
			<el-form :model="ruleForm" size="default" label-position="top">
				<el-row :gutter="35">
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl1="24" class="mb20">
						<el-form-item prop="dataScope" label="数据范围：">
							<el-select v-model="ruleForm.dataScope" placeholder="数据范围" style="width: 100%">
								<el-option v-for="d in dataScopeType" :key="d.value" :label="d.label" :value="d.value" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl1="24" v-show="ruleForm.dataScope === 5">
						<el-form-item prop="orgIdList" label="机构列表：">
							<OrgTree ref="orgTreeRef" class="w100" />
						</el-form-item>
					</el-col>
				</el-row>
			</el-form>
			<template #footer>
				<span class="dialog-footer">
					<el-button @click="cancel" size="default">取 消</el-button>
					<el-button type="primary" @click="submit" size="default">确 定</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>

<script lang="ts">
import { reactive, toRefs, defineComponent, ref } from 'vue';
import OrgTree from '/@/views/system/org/component/orgTree.vue';

import { getAPI } from '/@/utils/axios-utils';
import { SysRoleApi } from '/@/api-services/api';
import { RoleOrgInput } from '/@/api-services/models';

export default defineComponent({
	name: 'sysGrantData',
	components: { OrgTree },
	setup() {
		const orgTreeRef = ref();
		const state = reactive({
			isShowDialog: false,
			ruleForm: {} as RoleOrgInput,
			dataScopeType: [
				{ value: 1, label: '全部数据' },
				{ value: 2, label: '本部门及以下数据' },
				{ value: 3, label: '本部门数据' },
				{ value: 4, label: '仅本人数据' },
				{ value: 5, label: '自定义数据' },
			],
		});
		// 打开弹窗
		const openDialog = async (row: any) => {
			state.ruleForm = row;
			var res = await getAPI(SysRoleApi).apiSysRoleGetRoleOwnOrgGet(row.id);
			setTimeout(() => {
				orgTreeRef.value?.setCheckedKeys(res.data.result);
			}, 100);
			state.isShowDialog = true;
		};
		// 取消
		const cancel = () => {
			state.isShowDialog = false;
		};
		// 提交
		const submit = async () => {
			if (state.ruleForm.dataScope === 5) state.ruleForm.orgIdList = orgTreeRef.value?.getCheckedKeys();
			await getAPI(SysRoleApi).apiSysRoleGrantRoleDataScopePost(state.ruleForm);
			state.isShowDialog = false;
		};
		return {
			orgTreeRef,
			openDialog,
			cancel,
			submit,
			...toRefs(state),
		};
	},
});
</script>
