<template>
	<div class="sys-config-container">
		<el-dialog v-model="isShowDialog" draggable width="600px">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Edit /> </el-icon>
					<span> {{ title }} </span>
				</div>
			</template>
			<el-form :model="ruleForm" ref="ruleFormRef" size="default" label-width="80px">
				<el-row :gutter="35">
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="配置名称" prop="name" :rules="[{ required: true, message: '配置名称不能为空', trigger: 'blur' }]">
							<el-input v-model="ruleForm.name" placeholder="配置名称" clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="配置编码" prop="code" :rules="[{ required: true, message: '配置编码不能为空', trigger: 'blur' }]">
							<el-input v-model="ruleForm.code" placeholder="配置编码" clearable :disabled="ruleForm.sysFlag == 1" />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="值" prop="value" :rules="[{ required: true, message: '值不能为空', trigger: 'blur' }]">
							<el-select  v-model="ruleForm.value" placeholder="值" clearable filterable allow-create default-first-option style="width: 100%">
								<el-option label="True" value="True" />
								<el-option label="False" value="False" />
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="内置参数" prop="sysFlag" :rules="[{ required: true, message: '内置参数不能为空', trigger: 'blur' }]">
							<el-radio-group v-model="ruleForm.sysFlag" :disabled="ruleForm.sysFlag == 1 && ruleForm.id != undefined">
								<el-radio :label="1">是</el-radio>
								<el-radio :label="2">否</el-radio>
							</el-radio-group>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="分组编码" prop="groupCode">
							<el-input v-model="ruleForm.groupCode" placeholder="分组编码" clearable :disabled="ruleForm.sysFlag == 1" />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="排序">
							<el-input-number v-model="ruleForm.orderNo" placeholder="排序" class="w100" />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="备注">
							<el-input v-model="ruleForm.remark" placeholder="请输入备注内容" clearable type="textarea" />
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
import mittBus from '/@/utils/mitt';

import { getAPI } from '/@/utils/axios-utils';
import { SysConfigApi } from '/@/api-services/api';
import { UpdateConfigInput } from '/@/api-services/models';

export default defineComponent({
	name: 'sysEditConfig',
	components: {},
	props: {
		title: {
			type: String,
			default: '',
		},
	},
	setup() {
		const ruleFormRef = ref();
		const state = reactive({
			isShowDialog: false,
			ruleForm: {} as UpdateConfigInput,
		});
		// 打开弹窗
		const openDialog = (row: any) => {
			state.ruleForm = JSON.parse(JSON.stringify(row));
			state.isShowDialog = true;
		};
		// 关闭弹窗
		const closeDialog = () => {
			mittBus.emit('submitRefresh');
			state.isShowDialog = false;
		};
		// 取消
		const cancel = () => {
			state.isShowDialog = false;
		};
		// 提交
		const submit = () => {
			ruleFormRef.value.validate(async (valid: boolean) => {
				if (!valid) return;
				if (state.ruleForm.id != undefined && state.ruleForm.id > 0) {
					await getAPI(SysConfigApi).apiSysConfigUpdateConfigPost(state.ruleForm);
				} else {
					await getAPI(SysConfigApi).apiSysConfigAddConfigPost(state.ruleForm);
				}
				closeDialog();
			});
		};
		return {
			ruleFormRef,
			openDialog,
			closeDialog,
			cancel,
			submit,
			...toRefs(state),
		};
	},
});
</script>
