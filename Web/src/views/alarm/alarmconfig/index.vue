<template>
	<div class="sys-alarmconfig-container">
		<el-row :gutter="8" style="width: 100%">
			<el-card shadow="hover">
				<el-tabs>
					<el-tab-pane label="数据库信息">
						<el-form :model="ruleFormBase" ref="ruleFormBaseRef" size="default" label-width="100px">
							<el-row :gutter="35">

								<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
									<el-form-item label="启用" prop="enable">
										<el-select v-model="ruleFormBase.enable" placeholder="数据库类型" style="width: 100%">
											<el-option :label="true" :value="true" />
											<el-option :label="false" :value="false" />
										</el-select>
									</el-form-item>
								</el-col>

								<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
									<el-form-item prop="dbType" label="数据库类型">
										<el-select v-model="ruleFormBase.dbType" placeholder="数据库类型"
											style="width: 100%">
											<el-option v-for="d in dbTypes" :key="d.value" :label="d.label"
												:value="d.value" />
										</el-select>
									</el-form-item>
								</el-col>
								


								<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
									<el-form-item label="连接字符串" prop="phone"
										>
										<el-input v-model="ruleFormBase.connStr" placeholder="连接字符串"  show-password/>
									</el-form-item>
								</el-col>

								<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
									<el-form-item>
										<el-button icon="ele-SuccessFilled" type="primary" @click="save"
											v-auth="'alarmconfig:save'"> 保存配置 </el-button>
									</el-form-item>
								</el-col>
							</el-row>
						</el-form>
					</el-tab-pane>

				</el-tabs>
			</el-card>
		</el-row>
	</div>
</template>

<script lang="ts">
import { toRefs, reactive, defineComponent, ref, onMounted, watch } from 'vue';
import { getAPI } from '/@/utils/axios-utils';
import { ElMessageBox, UploadInstance } from 'element-plus';
import { AlarmApi } from '/@/api-services/api';
import { AlarmConfig } from '/@/api-services/models';

export default defineComponent({
	name: 'alarmconfig',
	components: {},
	setup() {
		const state = reactive({
			loading: false,
			ruleFormBase: {} as AlarmConfig,
			dbTypes: [
				{ value: 0, label: 'SqlServer' },
				{ value: 1, label: 'Mysql' },
				{ value: 2, label: 'Sqlite' },
				{ value: 3, label: 'PostgreSQL' },
				{ value: 4, label: 'Oracle' },
			],
		});
		onMounted(async () => {
			state.loading = true;
			var res = await getAPI(AlarmApi).apiAlarmGetAlarmConfigGet();
			state.ruleFormBase = res.data.result ?? {};
			state.loading = false;
		});

		const save = async () => {
			await getAPI(AlarmApi).apiAlarmUpdateAlarmConfigPost(state.ruleFormBase).then(async () => {
				ElMessageBox.alert('保存成功', '提示', {
					confirmButtonText: '确定',
					type: 'info',
				})
			})

		};


		return {
			save,
			...toRefs(state),
		};
	},
});
</script>
