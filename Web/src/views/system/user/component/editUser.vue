<template>
	<div class="sys-user-container">
		<el-dialog v-model="isShowDialog" draggable :close-on-click-modal="false" width="769px">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Edit /> </el-icon>
					<span>{{ title }}</span>
				</div>
			</template>
			<el-tabs v-loading="loading">
				<el-tab-pane label="基础信息">
					<el-form :model="ruleForm" ref="ruleFormRef" size="default" label-width="100px">
						<el-row :gutter="35">
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="账号名称" prop="account" :rules="[{ required: true, message: '账号名称不能为空', trigger: 'blur' }]">
									<el-input v-model="ruleForm.account" placeholder="账号名称" clearable />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="真实姓名" prop="realName" :rules="[{ required: true, message: '真实姓名不能为空', trigger: 'blur' }]">
									<el-input v-model="ruleForm.realName" placeholder="真实姓名" clearable />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="昵称" prop="nickName">
									<el-input v-model="ruleForm.nickName" placeholder="昵称" clearable />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="手机号码" prop="phone" >
									<el-input v-model="ruleForm.phone" placeholder="手机号码" clearable />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="出生日期" prop="birthday" :rules="[{ required: true, message: '出生日期不能为空', trigger: 'blur' }]">
									<el-date-picker v-model="ruleForm.birthday" type="date" placeholder="出生日期" format="YYYY-MM-DD" value-format="YYYY-MM-DD" style="width: 100%" />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="性别" prop="sex">
									<el-radio-group v-model="ruleForm.sex">
										<el-radio :label="1">男</el-radio>
										<el-radio :label="2">女</el-radio>
									</el-radio-group>
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12">
								<el-form-item label="角色" prop="roleIdList" :rules="[{ required: true, message: '角色集合不能为空', trigger: 'blur' }]">
									<el-select v-model="ruleForm.roleIdList" multiple value-key="id" clearable placeholder="角色集合" collapse-tags collapse-tags-tooltip class="w100" filterable>
										<el-option v-for="item in roleData" :key="item.id" :label="item.name" :value="item.id" />
									</el-select>
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb5">
								<el-form-item label="排序">
									<el-input-number v-model="ruleForm.orderNo" placeholder="排序" class="w100" />
								</el-form-item>
							</el-col>
							<el-divider border-style="dashed" content-position="center">
								<div style="color: #b1b3b8">机构组织</div>
							</el-divider>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="所属机构" prop="orgId"
								>
									<el-cascader :options="orgData" :props="{ checkStrictly: true, emitPath: false, value: 'id', label: 'name' }" 
									placeholder="所属机构" clearable class="w100" v-model="ruleForm.orgId">
										<template #default="{ node, data }">
											<span>{{ data.name }}</span>
											<span v-if="!node.isLeaf"> ({{ data.children.length }}) </span>
										</template>
									</el-cascader>
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="职位" prop="posId" 
								>
									<el-select v-model="ruleForm.posId" placeholder="职位" style="width: 100%">
										<el-option v-for="d in posData" :key="d.id" :label="d.name" :value="d.id" />
									</el-select>
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="工号" prop="jobNum">
									<el-input v-model="ruleForm.jobNum" placeholder="工号" clearable />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="入职日期" prop="joinDate">
									<el-date-picker v-model="ruleForm.joinDate" type="date" placeholder="入职日期" format="YYYY-MM-DD" value-format="YYYY-MM-DD" style="width: 100%" />
								</el-form-item>
							</el-col>
							<el-divider border-style="dashed" content-position="center">
								<div style="color: #b1b3b8">附属机构</div>
							</el-divider>
							<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
								<el-button icon="ele-Plus" type="primary" plain @click="addExtOrgRow"> 增加附属机构 </el-button>
								<span style="font-size: 12px; color: gray; padding-left: 5px"> 具有相应组织机构的数据权限 </span>
							</el-col>
							<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
								<template v-if="ruleForm.extOrgIdList != undefined && ruleForm.extOrgIdList.length > 0">
									<el-row :gutter="35" v-for="(v, k) in ruleForm.extOrgIdList" :key="k">
										<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
											<el-form-item label="机构" :prop="`extOrgIdList[${k}].orgId`" :rules="[{ required: true, message: `机构不能为空`, trigger: 'blur' }]">
												<template #label>
													<el-button icon="ele-Delete" type="danger" circle plain size="small" @click="deleteExtOrgRow(k)" />
													<span class="ml5">机构</span>
												</template>
												<el-cascader
													:options="orgData"
													:props="{ checkStrictly: true, emitPath: false, value: 'id', label: 'name' }"
													placeholder="机构组织"
													clearable
													class="w100"
													v-model="ruleForm.extOrgIdList[k].orgId"
												>
													<template #default="{ node, data }">
														<span>{{ data.name }}</span>
														<span v-if="!node.isLeaf"> ({{ data.children.length }}) </span>
													</template>
												</el-cascader>
											</el-form-item>
										</el-col>
										<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
											<el-form-item label="职位" :prop="`extOrgIdList[${k}].posId`" :rules="[{ required: true, message: `职位不能为空`, trigger: 'blur' }]">
												<el-select v-model="ruleForm.extOrgIdList[k].posId" placeholder="职位名称" style="width: 100%">
													<el-option v-for="d in posData" :key="d.id" :label="d.name" :value="d.id" />
												</el-select>
											</el-form-item>
										</el-col>
									</el-row>
								</template>
								<el-empty description="空" v-else></el-empty>
							</el-col>
						</el-row>
					</el-form>
				</el-tab-pane>
				<el-tab-pane label="档案信息">
					<el-form :model="ruleForm" size="default" label-width="100px">
						<el-row :gutter="35">
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="证件类型" prop="cardType">
									<el-select v-model="ruleForm.cardType" placeholder="证件类型" style="width: 100%">
										<el-option label="身份证" :value="0" />
										<el-option label="护照" :value="1" />
										<el-option label="出生证" :value="2" />
										<el-option label="港澳台通行证" :value="3" />
										<el-option label="外国人居留证" :value="4" />
									</el-select>
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="证件号码" prop="idCardNum">
									<el-input v-model="ruleForm.idCardNum" placeholder="证件号码" clearable />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="民族" prop="nation">
									<el-input v-model="ruleForm.nation" placeholder="民族" clearable />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="邮箱" prop="email">
									<el-input v-model="ruleForm.email" placeholder="邮箱" clearable />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
								<el-form-item label="地址" prop="nation">
									<el-input v-model="ruleForm.address" placeholder="地址" clearable type="textarea" />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="毕业学校" prop="college">
									<el-input v-model="ruleForm.college" placeholder="毕业学校" clearable />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="文化程度" prop="cultureLevel">
									<el-select v-model="ruleForm.cultureLevel" placeholder="文化程度" style="width: 100%">
										<el-option label="小学" :value="0" />
										<el-option label="初中" :value="1" />
										<el-option label="高中" :value="2" />
										<el-option label="中专" :value="3" />
										<el-option label="大专" :value="4" />
										<el-option label="本科" :value="5" />
										<el-option label="硕士研究生" :value="6" />
										<el-option label="博士研究生" :value="7" />
									</el-select>
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="政治面貌" prop="politicalOutlook">
									<el-input v-model="ruleForm.politicalOutlook" placeholder="政治面貌" clearable />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="办公电话" prop="officePhone">
									<el-input v-model="ruleForm.officePhone" placeholder="办公电话" clearable />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="紧急联系人" prop="emergencyContact">
									<el-input v-model="ruleForm.emergencyContact" placeholder="紧急联系人" clearable />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
								<el-form-item label="联系人电话" prop="emergencyPhone">
									<el-input v-model="ruleForm.emergencyPhone" placeholder="联系人电话" clearable />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
								<el-form-item label="联系人地址" prop="emergencyAddress">
									<el-input v-model="ruleForm.emergencyAddress" placeholder="紧急联系人" clearable type="textarea" />
								</el-form-item>
							</el-col>
							<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
								<el-form-item label="备注" prop="remark">
									<el-input v-model="ruleForm.remark" placeholder="备注" clearable type="textarea" />
								</el-form-item>
							</el-col>
						</el-row>
					</el-form>
				</el-tab-pane>
			</el-tabs>
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
import { reactive, toRefs, defineComponent, ref, onMounted } from 'vue';
import mittBus from '/@/utils/mitt';

import { getAPI } from '/@/utils/axios-utils';
import { SysPosApi, SysRoleApi, SysUserApi } from '/@/api-services/api';
import { RoleOutput, SysOrg, SysPos, UpdateUserInput } from '/@/api-services/models';

export default defineComponent({
	name: 'sysEditUser',
	components: {},
	props: {
		title: {
			type: String,
			default: '',
		},
		orgData: {
			type: Array<SysOrg>,
			default: () => [],
		},
	},
	setup() {
		const ruleFormRef = ref();
		const state = reactive({
			loading: false,
			isShowDialog: false,
			ruleForm: {} as UpdateUserInput,
			posData: [] as Array<SysPos>, // 职位数据
			roleData: [] as Array<RoleOutput>, // 角色数据
		});
		onMounted(async () => {
			state.loading = true;
			var res = await getAPI(SysPosApi).apiSysPosGetPosListGet();
			state.posData = res.data.result ?? [];
			var res1 = await getAPI(SysRoleApi).apiSysRoleGetRoleListGet();
			state.roleData = res1.data.result ?? [];
			state.loading = false;
		});
		// 打开弹窗
		const openDialog = async (row: any) => {
			state.ruleForm = JSON.parse(JSON.stringify(row));
			if (JSON.stringify(row) !== '{}') {
				var resRole = await getAPI(SysUserApi).apiSysUserGetUserOwnRoleUserIdGet(row.id);
				state.ruleForm.roleIdList = resRole.data.result;
				var resExtOrg = await getAPI(SysUserApi).apiSysUserGetUserOrgListUserIdGet(row.id);
				state.ruleForm.extOrgIdList = resExtOrg.data.result;
				state.isShowDialog = true;
			} else state.isShowDialog = true;
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
					await getAPI(SysUserApi).apiSysUserUpdateUserPost(state.ruleForm);
				} else {
					await getAPI(SysUserApi).apiSysUserAddUserPost(state.ruleForm);
				}
				closeDialog();
			});
		};
		// 增加附属机构行
		const addExtOrgRow = () => {
			if (state.ruleForm.extOrgIdList == undefined) state.ruleForm.extOrgIdList = [];
			state.ruleForm.extOrgIdList?.push({});
		};
		// 删除附属机构行
		const deleteExtOrgRow = (k: number) => {
			state.ruleForm.extOrgIdList?.splice(k, 1);
		};
		return {
			ruleFormRef,
			openDialog,
			closeDialog,
			cancel,
			submit,
			addExtOrgRow,
			deleteExtOrgRow,
			...toRefs(state),
		};
	},
});
</script>
