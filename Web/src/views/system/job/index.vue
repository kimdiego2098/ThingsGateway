<template>
	<div class="sys-job-container">
		<el-card shadow="hover" :body-style="{ paddingBottom: '0' }">
			<el-form :model="queryParams" ref="queryForm" :inline="true">
				<el-form-item label="作业编号" prop="jobId">
					<el-input placeholder="作业编号" clearable @keyup.enter="handleQuery" v-model="queryParams.jobId" />
				</el-form-item>
				<el-form-item label="描述信息" prop="description">
					<el-input placeholder="描述信息" clearable @keyup.enter="handleQuery" v-model="queryParams.description" />
				</el-form-item>
				<el-form-item>
					<el-button icon="ele-Refresh" @click="resetQuery"> 重置 </el-button>
					<el-button type="primary" icon="ele-Search" @click="handleQuery" v-auth="'sysJob:page'"> 查询 </el-button>
					<el-button-group style="margin: 0px 12px">
						<el-tooltip content="增加作业">
							<el-button icon="ele-CirclePlus" @click="openAddJobDetail" v-auth="'sysJob:add'"> </el-button>
						</el-tooltip>
						<el-tooltip content="启动所有作业">
							<el-button icon="ele-VideoPlay" @click="startAllJob" />
						</el-tooltip>
						<el-tooltip content="暂停所有作业">
							<el-button icon="ele-VideoPause" @click="pauseAllJob" />
						</el-tooltip>
					</el-button-group>
					<el-button-group style="margin: 0px 12px 0px 0px">
						<el-tooltip content="强制唤醒作业调度器">
							<el-button icon="ele-AlarmClock" @click="cancelSleep" />
						</el-tooltip>
						<el-tooltip content="强制触发所有作业持久化">
							<el-button icon="ele-Connection" @click="persistAll" />
						</el-tooltip>
					</el-button-group>
					<el-button icon="ele-Coin" @click="openJobCluster" plain> 集群控制 </el-button>
					<el-button icon="ele-Grid" @click="openJobDashboard" plain> 任务看板 </el-button>
				</el-form-item>
			</el-form>
		</el-card>

		<el-card shadow="hover" style="margin-top: 8px">
			<el-table :data="jobData" style="width: 100%" v-loading="loading" border>
				<el-table-column type="expand" fixed>
					<template #default="scope">
						<el-table :data="scope.row.jobTriggers" border size="small">
							<el-table-column type="index" label="序号" width="55" align="center" fixed />
							<el-table-column prop="triggerId" label="触发器编号" width="120" fixed show-overflow-tooltip />
							<el-table-column prop="triggerType" label="类型" show-overflow-tooltip />
							<el-table-column prop="assemblyName" label="程序集" show-overflow-tooltip />
							<el-table-column prop="args" label="参数" show-overflow-tooltip />
							<el-table-column prop="description" label="描述" width="120" show-overflow-tooltip />
							<el-table-column prop="status" label="状态" width="100" align="center" show-overflow-tooltip>
								<template #default="scope">
									<el-tag v-if="scope.row.status == 0"> 积压 </el-tag>
									<el-tag v-if="scope.row.status == 1"> 就绪 </el-tag>
									<el-tag v-if="scope.row.status == 2"> 正在运行 </el-tag>
									<el-tag v-if="scope.row.status == 3"> 暂停 </el-tag>
									<el-tag v-if="scope.row.status == 4"> 阻塞 </el-tag>
									<el-tag v-if="scope.row.status == 5"> 由失败进入就绪 </el-tag>
									<el-tag v-if="scope.row.status == 6"> 归档 </el-tag>
									<el-tag v-if="scope.row.status == 7"> 崩溃 </el-tag>
									<el-tag v-if="scope.row.status == 8"> 超限 </el-tag>
									<el-tag v-if="scope.row.status == 9"> 无触发时间 </el-tag>
									<el-tag v-if="scope.row.status == 10"> 未启动 </el-tag>
									<el-tag v-if="scope.row.status == 11"> 未知作业触发器 </el-tag>
									<el-tag v-if="scope.row.status == 12"> 未知作业处理程序 </el-tag>
								</template>
							</el-table-column>
							<el-table-column prop="startTime" label="起始时间" width="100" align="center" show-overflow-tooltip />
							<el-table-column prop="endTime" label="结束时间" width="100" align="center" show-overflow-tooltip />
							<el-table-column prop="lastRunTime" label="最近运行时间" width="140" align="center" show-overflow-tooltip />
							<el-table-column prop="nextRunTime" label="下一次运行时间" width="140" align="center" show-overflow-tooltip />
							<el-table-column prop="numberOfRuns" label="触发次数" width="100" align="center" show-overflow-tooltip />
							<el-table-column prop="maxNumberOfRuns" label="最大触发次数" width="120" align="center" show-overflow-tooltip />
							<el-table-column prop="numberOfErrors" label="出错次数" width="100" align="center" show-overflow-tooltip />
							<el-table-column prop="maxNumberOfErrors" label="最大出错次数" width="120" align="center" show-overflow-tooltip />
							<el-table-column prop="numRetries" label="重试次数" width="100" align="center" show-overflow-tooltip />
							<el-table-column prop="retryTimeout" label="重试间隔ms" width="100" align="center" show-overflow-tooltip />
							<el-table-column prop="startNow" label="是否立即启动" width="100" align="center" show-overflow-tooltip>
								<template #default="scope">
									<el-tag v-if="scope.row.startNow == true"> 是 </el-tag>
									<el-tag v-else> 否 </el-tag>
								</template>
							</el-table-column>
							<el-table-column prop="runOnStart" label="是否启动时执行一次" width="150" align="center" show-overflow-tooltip>
								<template #default="scope">
									<el-tag v-if="scope.row.runOnStart == true"> 是 </el-tag>
									<el-tag v-else> 否 </el-tag>
								</template>
							</el-table-column>
							<el-table-column prop="resetOnlyOnce" label="是否只运行一次" width="120" align="center" show-overflow-tooltip>
								<template #default="scope">
									<el-tag v-if="scope.row.resetOnlyOnce == true"> 是 </el-tag>
									<el-tag v-else> 否 </el-tag>
								</template>
							</el-table-column>
							<el-table-column prop="updatedTime" label="更新时间" width="140" align="center" show-overflow-tooltip />
							<el-table-column label="操作" width="140" align="center" show-overflow-tooltip>
								<template #default="scope">
									<el-tooltip content="启动触发器">
										<el-button size="small" type="primary" icon="ele-VideoPlay" text @click="startTrigger(scope.row)" />
									</el-tooltip>
									<el-tooltip content="暂停触发器">
										<el-button size="small" type="primary" icon="ele-VideoPause" text @click="pauseTrigger(scope.row)" />
									</el-tooltip>
									<el-tooltip content="编辑触发器">
										<el-button size="small" type="primary" icon="ele-Edit" text @click="openEditJobTrigger(scope.row)"> </el-button>
									</el-tooltip>
									<el-tooltip content="删除触发器">
										<el-button size="small" type="danger" icon="ele-Delete" text @click="delJobTrigger(scope.row)"> </el-button>
									</el-tooltip>
								</template>
							</el-table-column>
						</el-table>
					</template>
				</el-table-column>
				<el-table-column type="index" label="序号" width="55" align="center" fixed />
				<el-table-column prop="jobDetail.jobId" label="作业编号" width="150" fixed>
					<template #default="scope">
						<div style="display: flex; align-items: center">
							<el-icon><timer /></el-icon>
							<span style="margin-left: 5px">{{ scope.row.jobDetail.jobId }}</span>
						</div>
					</template>
				</el-table-column>
				<el-table-column prop="jobDetail.groupName" label="组名称" show-overflow-tooltip />
				<el-table-column prop="jobDetail.jobType" label="类型" show-overflow-tooltip />
				<el-table-column prop="jobDetail.assemblyName" label="程序集" show-overflow-tooltip />
				<el-table-column prop="jobDetail.description" label="描述" show-overflow-tooltip />
				<el-table-column prop="jobDetail.concurrent" label="执行方式" width="100" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-tag v-if="scope.row.jobDetail.concurrent == true"> 并行 </el-tag>
						<el-tag v-else> 串行 </el-tag>
					</template>
				</el-table-column>
				<el-table-column prop="jobDetail.includeAnnotations" label="扫描特性触发器" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-tag v-if="scope.row.includeAnnotations == true"> 是 </el-tag>
						<el-tag v-else> 否 </el-tag>
					</template>
				</el-table-column>
				<el-table-column prop="jobDetail.properties" label="额外数据" show-overflow-tooltip />
				<el-table-column prop="jobDetail.updatedTime" label="更新时间" width="160" align="center" show-overflow-tooltip />
				<el-table-column label="操作" width="170" fixed="right" align="center" show-overflow-tooltip>
					<template #default="scope">
						<el-tooltip content="增加触发器">
							<el-button size="small" type="primary" icon="ele-CirclePlus" text @click="openAddJobTrigger(scope.row)"> </el-button>
						</el-tooltip>
						<el-tooltip content="启动作业">
							<el-button size="small" type="primary" icon="ele-VideoPlay" text @click="startJob(scope.row)" />
						</el-tooltip>
						<el-tooltip content="暂停作业">
							<el-button size="small" type="primary" icon="ele-VideoPause" text @click="pauseJob(scope.row)" />
						</el-tooltip>
						<el-tooltip content="编辑作业">
							<el-button size="small" type="primary" icon="ele-Edit" text @click="openEditJobDetail(scope.row)" v-auth="'sysJob:update'"> </el-button>
						</el-tooltip>
						<el-tooltip content="删除作业">
							<el-button size="small" type="danger" icon="ele-Delete" text @click="delJobDetail(scope.row)" v-auth="'sysJob:delete'"> </el-button>
						</el-tooltip>
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
		<EditJobDetail ref="editJobDetailRef" :title="editJobDetailTitle" />
		<EditJobTrigger ref="editJobTriggerRef" :title="editJobTriggerTitle" />
		<JobCluster ref="editJobClusterRef" />
	</div>
</template>

<script lang="ts">
import { toRefs, reactive, onMounted, ref, defineComponent, onUnmounted } from 'vue';
import { ElMessageBox, ElMessage } from 'element-plus';
import mittBus from '/@/utils/mitt';
import { Timer } from '@element-plus/icons-vue';
import EditJobDetail from '/@/views/system/job/component/editJobDetail.vue';
import EditJobTrigger from '/@/views/system/job/component/editJobTrigger.vue';
import JobCluster from '/@/views/system/job/component/jobCluster.vue';
import { useRouter } from 'vue-router';
import { getAPI } from '/@/utils/axios-utils';
import { SysJobApi } from '/@/api-services/api';
import { JobOutput } from '/@/api-services/models';

export default defineComponent({
	name: 'sysJob',
	components: { Timer, EditJobDetail, EditJobTrigger, JobCluster },
	setup() {
		const router = useRouter();
		const editJobDetailRef = ref();
		const editJobTriggerRef = ref();
		const editJobClusterRef = ref();
		const state = reactive({
			loading: false,
			jobData: [] as Array<JobOutput>,
			queryParams: {
				jobId: undefined,
				description: undefined,
			},
			tableParams: {
				page: 1,
				pageSize: 10,
				total: 0 as any,
			},
			editJobDetailTitle: '',
			editJobTriggerTitle: '',
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
			var res = await getAPI(SysJobApi).apiSysJobGetJobPageGet(state.queryParams.jobId, state.queryParams.description, state.tableParams.page, state.tableParams.pageSize);
			state.jobData = res.data.result?.items ?? [];
			state.tableParams.total = res.data.result?.total;
			state.loading = false;
		};
		// 重置操作
		const resetQuery = () => {
			state.queryParams.jobId = undefined;
			state.queryParams.description = undefined;
			handleQuery();
		};
		// 打开新增作业页面
		const openAddJobDetail = () => {
			state.editJobDetailTitle = '添加作业';
			editJobDetailRef.value.openDialog({ concurrent: true, includeAnnotations: true, groupName: 'default' });
		};
		// 打开编辑作业页面
		const openEditJobDetail = (row: any) => {
			state.editJobDetailTitle = '编辑作业';
			editJobDetailRef.value.openDialog(row.jobDetail);
		};
		// 删除作业
		const delJobDetail = (row: any) => {
			ElMessageBox.confirm(`确定删除作业：【${row.jobDetail.jobId}】?`, '提示', {
				confirmButtonText: '确定',
				cancelButtonText: '取消',
				type: 'warning',
			})
				.then(async () => {
					await getAPI(SysJobApi).apiSysJobDeleteJobDetailPost({ jobId: row.jobDetail.jobId });
					handleQuery();
					ElMessage.success('删除成功');
				})
				.catch(() => {});
		};
		// 打开新增触发器页面
		const openAddJobTrigger = (row: any) => {
			state.editJobTriggerTitle = '添加触发器';
			editJobTriggerRef.value.openDialog({ jobId: row.jobDetail.jobId, retryTimeout: 1000, startNow: true, runOnStart: true, resetOnlyOnce: true, triggerType: 'Furion.Schedule.PeriodTrigger' });
		};
		// 打开编辑触发器页面
		const openEditJobTrigger = (row: any) => {
			state.editJobTriggerTitle = '编辑触发器';
			editJobTriggerRef.value.openDialog(row);
		};
		// 删除触发器
		const delJobTrigger = (row: any) => {
			ElMessageBox.confirm(`确定删除触发器：【${row.triggerId}】?`, '提示', {
				confirmButtonText: '确定',
				cancelButtonText: '取消',
				type: 'warning',
			})
				.then(async () => {
					await getAPI(SysJobApi).apiSysJobDeleteJobTriggerPost({ triggerId: row.triggerId });
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
		// 启动所有作业
		const startAllJob = async () => {
			await getAPI(SysJobApi).apiSysJobStartAllJobPost();
			ElMessage.success('启动所有作业');
		};
		// 暂停所有作业
		const pauseAllJob = async () => {
			await getAPI(SysJobApi).apiSysJobPauseAllJobPost();
			ElMessage.success('暂停所有作业');
		};
		// 启动某个作业
		const startJob = async (row: any) => {
			await getAPI(SysJobApi).apiSysJobStartJobPost({ jobId: row.jobDetail.jobId });
			ElMessage.success('启动作业');
		};
		// 暂停某个作业
		const pauseJob = async (row: any) => {
			await getAPI(SysJobApi).apiSysJobPauseJobPost({ jobId: row.jobDetail.jobId });
			ElMessage.success('暂停作业');
		};
		// 启动触发器
		const startTrigger = async (row: any) => {
			await getAPI(SysJobApi).apiSysJobStartTriggerPost({ jobId: row.jobId, triggerId: row.triggerId });
			ElMessage.success('启动触发器');
		};
		// 暂停触发器
		const pauseTrigger = async (row: any) => {
			await getAPI(SysJobApi).apiSysJobPauseTriggerPost({ jobId: row.jobId, triggerId: row.triggerId });
			ElMessage.success('暂停触发器');
		};
		// 强制唤醒作业调度器
		const cancelSleep = async () => {
			await getAPI(SysJobApi).apiSysJobCancelSleepPost();
			ElMessage.success('强制唤醒作业调度器');
		};
		// 强制触发所有作业持久化
		const persistAll = async () => {
			await getAPI(SysJobApi).apiSysJobPersistAllPost();
			ElMessage.success('强制触发所有作业持久化');
		};
		// 打开集群控制页面
		const openJobCluster = () => {
			editJobClusterRef.value.openDrawer();
		};
			// 打开任务看板
			const openJobDashboard = () => {
			router.push({
				path: '/platform/job/dashboard',
			});
		};
		return {
			editJobDetailRef,
			editJobTriggerRef,
			editJobClusterRef,
			handleQuery,
			resetQuery,
			openAddJobDetail,
			openEditJobDetail,
			delJobDetail,
			handleSizeChange,
			handleCurrentChange,
			openAddJobTrigger,
			openEditJobTrigger,
			delJobTrigger,
			startAllJob,
			pauseAllJob,
			startJob,
			pauseJob,
			startTrigger,
			pauseTrigger,
			cancelSleep,
			persistAll,
			openJobCluster,
			openJobDashboard,
			...toRefs(state),
		};
	},
});
</script>
