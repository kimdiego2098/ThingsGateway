<template>
	<div class="sys-jobTrigger-container">
		<el-dialog v-model="isShowDialog" draggable :close-on-click-modal="false" width="769px">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Edit /> </el-icon>
					<span> {{ title }} </span>
				</div>
			</template>
			<el-form :model="ruleForm" ref="ruleFormRef" size="default" label-width="110px">
				<el-row :gutter="35">
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="触发器编号" prop="triggerId" :rules="[{ required: true, message: '触发器编号不能为空', trigger: 'blur' }]">
							<el-input v-model="ruleForm.triggerId" placeholder="触发器编号" clearable />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="触发器类型">
							<el-select v-model="ruleForm.triggerType" style="width: 100%">
								<el-option value="Furion.Schedule.PeriodTrigger" label="间隔"></el-option>
								<el-option value="Furion.Schedule.CronTrigger" label="Cron表达式"></el-option>
							</el-select>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20" v-if="ruleForm.triggerType == 'Furion.Schedule.PeriodTrigger'">
						<el-form-item label="参数">
							<el-input-number v-model="ruleForm.args" placeholder="间隔" class="w100" />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20" v-else>
						<el-form-item label="参数">
							<el-input v-model="ruleForm.args" placeholder="Cron表达式">
								<template #append>
									<el-button @click="showCronDialog = true">Cron表达式</el-button>
								</template>
							</el-input>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="起始时间">
							<el-date-picker v-model="ruleForm.startTime" type="datetime" placeholder="起始时间" style="width: 100%" format="YYYY-MM-DD HH:mm:ss" value-format="YYYY-MM-DD HH:mm:ss" />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="结束时间">
							<el-date-picker v-model="ruleForm.endTime" type="datetime" placeholder="结束时间" style="width: 100%" format="YYYY-MM-DD HH:mm:ss" value-format="YYYY-MM-DD HH:mm:ss" />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="最大触发次数">
							<el-input-number v-model="ruleForm.maxNumberOfRuns" placeholder="最大触发次数" class="w100" />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="最大出错次数">
							<el-input-number v-model="ruleForm.maxNumberOfErrors" placeholder="最大出错次数" class="w100" />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="重试次数">
							<el-input-number v-model="ruleForm.numRetries" placeholder="重试次数" class="w100" />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="重试间隔(ms)">
							<el-input-number v-model="ruleForm.retryTimeout" placeholder="重试间隔ms" class="w100" />
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="立即启动">
							<el-radio-group v-model="ruleForm.startNow">
								<el-radio :label="true">是</el-radio>
								<el-radio :label="false">否</el-radio>
							</el-radio-group>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="启动时执行一次">
							<el-radio-group v-model="ruleForm.runOnStart">
								<el-radio :label="true">是</el-radio>
								<el-radio :label="false">否</el-radio>
							</el-radio-group>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="仅执行一次">
							<el-radio-group v-model="ruleForm.resetOnlyOnce">
								<el-radio :label="true">是</el-radio>
								<el-radio :label="false">否</el-radio>
							</el-radio-group>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="描述信息" prop="description">
							<el-input v-model="ruleForm.description" placeholder="描述信息" clearable type="textarea" />
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

		<el-dialog v-model="showCronDialog" draggable :close-on-click-modal="false" class="scrollbar">
			<template #header>
				<div style="color: #fff">
					<el-icon size="16" style="margin-right: 3px; display: inline; vertical-align: middle"> <ele-Edit /> </el-icon>
					<span> Cron表达式生成器 </span>
				</div>
			</template>
			<cronTab @hide="showCronDialog = false" @fill="crontabFill" :expression="ruleForm.args"></cronTab>
		</el-dialog>
	</div>
</template>

<script lang="ts">
import { reactive, toRefs, defineComponent, ref } from 'vue';
import mittBus from '/@/utils/mitt';

import cronTab from './cronTab/index.vue';
import { getAPI } from '/@/utils/axios-utils';
import { SysJobApi } from '/@/api-services/api';
import { UpdateJobTriggerInput } from '/@/api-services/models';

export default defineComponent({
	name: 'sysEditJobTrigger',
	components: { cronTab },
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
			ruleForm: {} as UpdateJobTriggerInput,
			showCronDialog: false,
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
					await getAPI(SysJobApi).apiSysJobUpdateJobTriggerPost(state.ruleForm);
				} else {
					await getAPI(SysJobApi).apiSysJobAddJobTriggerPost(state.ruleForm);
				}
				closeDialog();
			});
		};
		// cron窗体确定后值
		const crontabFill = (value: string | null | undefined) => {
			state.ruleForm.args = value;
		};

		return {
			ruleFormRef,
			openDialog,
			closeDialog,
			cancel,
			submit,
			crontabFill,
			...toRefs(state),
		};
	},
});
</script>
