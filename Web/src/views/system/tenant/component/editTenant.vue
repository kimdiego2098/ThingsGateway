<template>
	<div class="sys-tenant-container">
		<el-dialog v-model="isShowDialog" draggable :close-on-click-modal="false" width="769px">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Edit /> </el-icon>
					<span> {{ title }} </span>
				</div>
			</template>
			<el-form :model="ruleForm" ref="ruleFormRef" size="default" label-width="100px">
				<el-row :gutter="35">
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="租户类型" :rules="[{ required: true, message: '租户类型不能为空', trigger: 'blur' }]">
							<el-radio-group v-model="ruleForm.tenantType" :disabled="ruleForm.id != undefined">
								<el-radio :label="0">ID隔离</el-radio>
								<el-radio :label="1">库隔离</el-radio>
							</el-radio-group>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="租户名称" prop="name" :rules="[{ required: true, message: '租户名称不能为空', trigger: 'blur' }]">
							<el-input v-model="ruleForm.name" placeholder="租户名称" clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="管理员" prop="adminName" :rules="[{ required: true, message: '管理员不能为空', trigger: 'blur' }]">
							<el-input v-model="ruleForm.adminName" placeholder="管理员" clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="电话" prop="phone" :rules="[{ required: true, message: '电话号码不能为空', trigger: 'blur' }]">
							<el-input v-model="ruleForm.phone" placeholder="电话" clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="邮箱">
							<el-input v-model="ruleForm.email" placeholder="邮箱" clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="排序">
							<el-input-number v-model="ruleForm.orderNo" placeholder="排序" class="w100" />
						</el-form-item>
					</el-col>
					<!-- <el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="主机">
							<el-input v-model="ruleForm.host" placeholder="主机" clearable />
						</el-form-item>
					</el-col> -->
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="数据库类型">
							<el-select v-model="ruleForm.dbType" placeholder="数据库类型" clearable style="width: 100%" :disabled="ruleForm.tenantType == 0 && ruleForm.tenantType != undefined">
								<el-option label="MySql" :value="0" />
								<el-option label="SqlServer" :value="1" />
								<el-option label="Sqlite" :value="2" />
								<el-option label="Oracle" :value="3" />
								<el-option label="PostgreSQL" :value="4" />
								<el-option label="Dm" :value="5" />
								<el-option label="Kdbndp" :value="6" />
								<el-option label="Oscar" :value="7" />
								<el-option label="MySqlConnector" :value="8" />
								<el-option label="Access" :value="9" />
								<el-option label="OpenGauss" :value="10" />
								<el-option label="QuestDB" :value="11" />
								<el-option label="HG" :value="12" />
								<el-option label="ClickHouse" :value="13" />
								<el-option label="GBase" :value="14" />
								<el-option label="Custom" :value="900" />
							</el-select>
						</el-form-item>
					</el-col>
					<!-- <el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="数据库标识">
							<el-input v-model="ruleForm.configId" placeholder="数据库标识" clearable :disabled="ruleForm.tenantType == 0 && ruleForm.tenantType != undefined" />
						</el-form-item>
					</el-col> -->
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="连接字符串">
							<el-input v-model="ruleForm.connection" placeholder="连接字符串" clearable type="textarea" :disabled="ruleForm.tenantType == 0 && ruleForm.tenantType != undefined" />
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
import { SysTenantApi } from '/@/api-services/api';
import { UpdateTenantInput } from '/@/api-services/models';

export default defineComponent({
	name: 'sysEditTenant',
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
			ruleForm: {} as UpdateTenantInput,
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
					await getAPI(SysTenantApi).apiSysTenantUpdateTenantPost(state.ruleForm);
				} else {
					await getAPI(SysTenantApi).apiSysTenantAddTenantPost(state.ruleForm);
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
