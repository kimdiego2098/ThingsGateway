<template>
	<div class="sys-server-container">
		<el-row :gutter="8">
			<el-col :md="12" :sm="24">
				<el-card shadow="hover" header="系统信息">
					<table class="sysInfo_table">
						<tr>
							<td class="sysInfo_td">主机名称：</td>
							<td class="sysInfo_td">{{ machineBaseInfo.hostName }}</td>
						</tr>
						<tr>
							<td class="sysInfo_td">操作系统：</td>
							<td class="sysInfo_td">{{ machineBaseInfo.systemOs }}</td>
						</tr>
						<tr>
							<td class="sysInfo_td">系统架构：</td>
							<td class="sysInfo_td">{{ machineBaseInfo.osArchitecture }}</td>
						</tr>
						<tr>
							<td class="sysInfo_td">CPU核数：</td>
							<td class="sysInfo_td">{{ machineBaseInfo.processorCount }}</td>
						</tr>
						<tr>
							<td class="sysInfo_td">运行时长：</td>
							<td class="sysInfo_td">{{ machineBaseInfo.sysRunTime }}</td>
						</tr>
						<tr>
							<td class="sysInfo_td">外网地址：</td>
							<td class="sysInfo_td">{{ machineBaseInfo.remoteIp }}</td>
						</tr>
						<tr>
							<td class="sysInfo_td">内网地址：</td>
							<td class="sysInfo_td">{{ machineBaseInfo.localIp }}</td>
						</tr>
						<tr>
							<td class="sysInfo_td">运行框架：</td>
							<td class="sysInfo_td">{{ machineBaseInfo.frameworkDescription }}</td>
						</tr>
					</table>
				</el-card>
			</el-col>
			<el-col :md="12" :sm="24">
				<el-card shadow="hover" header="使用信息">
					<el-row>
						<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" style="text-align: center">
							<el-progress
								type="dashboard"
								:percentage="parseInt(machineUseInfo.ramRate == undefined ? 0 : machineUseInfo.ramRate.substr(0, machineUseInfo.ramRate.length - 1))"
								:color="'var(--el-color-primary)'"
							>
								<template #default>
									<span>{{ machineUseInfo.ramRate }}<br /></span>
									<span style="font-size: 10px">
										已用:{{ machineUseInfo.usedRam }}<br />
										剩余:{{ machineUseInfo.freeRam }}<br />
										内存使用率
									</span>
								</template>
							</el-progress>
						</el-col>
						<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" style="text-align: center">
							<el-progress
								type="dashboard"
								:percentage="parseInt(machineUseInfo.cpuRate == undefined ? 0 : machineUseInfo.cpuRate.substr(0, machineUseInfo.cpuRate.length - 1))"
								:color="'var(--el-color-primary)'"
							>
								<template #default>
									<span>{{ machineUseInfo.cpuRate }}<br /></span>
									<span style="font-size: 10px"> CPU使用率 </span>
								</template>
							</el-progress>
						</el-col>
					</el-row>

					<table class="sysInfo_table">
						<tr>
							<td class="sysInfo_td">启动时间：</td>
							<td class="sysInfo_td">{{ machineUseInfo.startTime }}</td>
						</tr>
						<tr>
							<td class="sysInfo_td">运行时长：</td>
							<td class="sysInfo_td">{{ machineUseInfo.runTime }}</td>
						</tr>
						<tr>
							<td class="sysInfo_td">网站目录：</td>
							<td class="sysInfo_td">{{ machineBaseInfo.wwwroot }}</td>
						</tr>
						<tr>
							<td class="sysInfo_td">开发环境：</td>
							<td class="sysInfo_td">{{ machineBaseInfo.environment }}</td>
						</tr>
						<tr>
							<td class="sysInfo_td">环境变量：</td>
							<td class="sysInfo_td">{{ machineBaseInfo.stage }}</td>
						</tr>
					</table>
				</el-card>
			</el-col>
		</el-row>
		<el-card shadow="hover" header="程序集信息" style="margin-top: 8px">
			<div v-for="d in assemblyInfo" :key="d.name" style="display: inline-flex; margin: 4px">
				<el-tag round>
					<div style="display: inline-flex">
						<div style="">{{ d.name }}</div>
						<div style="font-size: 9px; margin-left: 3px">{{ d.version }}</div>
					</div>
				</el-tag>
			</div>
		</el-card>
		<el-card shadow="hover" header="磁盘信息" style="margin-top: 8px">
			<el-row>
				<el-col
					:span="4"
					:xs="24"
					:sm="24 / machineDiskInfo.length"
					:md="24 / machineDiskInfo.length"
					:lg="24 / machineDiskInfo.length"
					:xl="24 / machineDiskInfo.length"
					v-for="d in machineDiskInfo"
					:key="d.diskName"
					style="text-align: center"
				>
					<el-progress type="circle" :percentage="d.availablePercent" :color="'var(--el-color-primary)'">
						<template #default>
							<span>{{ d.availablePercent }}%<br /></span>
							<span style="font-size: 10px">
								已用:{{ d.used }}GB<br />
								剩余:{{ d.availableFreeSpace }}GB<br />
								{{ d.diskName }}
							</span>
						</template>
					</el-progress>
				</el-col>
			</el-row>
		</el-card>
	</div>
</template>

<script lang="ts">
import { toRefs, reactive, defineComponent, onActivated, onDeactivated, onMounted } from 'vue';

import { getAPI } from '/@/utils/axios-utils';
import { SysServerApi } from '/@/api-services';

export default defineComponent({
	name: 'sysServer',
	components: {},
	setup() {
		const state = reactive({
			machineBaseInfo: [] as any,
			machineUseInfo: [] as any,
			machineDiskInfo: [] as any,
			assemblyInfo: [] as any,
			timer: null as any,
		});
		onMounted(async () => {
			loadMachineBaseInfo();
			loadMachineUseInfo();
			loadMachineDiskInfo();
			loadAssemblyInfo();
		});
		// 服务器配置信息
		const loadMachineBaseInfo = async () => {
			var res = await getAPI(SysServerApi).apiSysServerGetServerBaseGet();
			state.machineBaseInfo = res.data.result;
		};
		// 服务器内存信息
		const loadMachineUseInfo = async () => {
			var res = await getAPI(SysServerApi).apiSysServerGetServerUsedGet();
			state.machineUseInfo = res.data.result;
		};
		// 服务器磁盘信息
		const loadMachineDiskInfo = async () => {
			var res = await getAPI(SysServerApi).apiSysServerGetServerDiskGet();
			state.machineDiskInfo = res.data.result;
		};
		// 框架程序集信息
		const loadAssemblyInfo = async () => {
			var res = await getAPI(SysServerApi).apiSysServerGetAssemblyGet();
			state.assemblyInfo = res.data.result;
		};
		// 实时刷新内存
		const refreshData = () => {
			loadMachineUseInfo();
		};
		onActivated(() => {
			state.timer = setInterval(() => {
				refreshData();
			}, 10000);
		});
		onDeactivated(() => {
			clearInterval(state.timer);
		});
		return {
			loadMachineBaseInfo,
			loadMachineUseInfo,
			loadMachineDiskInfo,
			loadAssemblyInfo,
			refreshData,
			...toRefs(state),
		};
	},
});
</script>

<style lang="scss">
.sysInfo_table {
	width: 100%;
	min-height: 40px;
	line-height: 40px;
	text-align: center;
}

.sysInfo_td {
	border-bottom: 1px solid #e8e8e8;
}
</style>
