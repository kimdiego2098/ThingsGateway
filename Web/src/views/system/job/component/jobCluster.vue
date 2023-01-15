<template>
	<div class="sys-jobCluster-container">
		<el-drawer v-model="state.isVisible" title="作业集群" size="30%">
			<el-card shadow="hover" style="margin: 8px">
				<el-table :data="state.jobClusterList" style="width: 100%" v-loading="state.loading" border>
					<el-table-column type="index" label="序号" width="55" align="center" />
					<el-table-column prop="clusterId" label="集群编号" show-overflow-tooltip />
					<el-table-column prop="status" label="状态" align="center" show-overflow-tooltip>
						<template #default="scope">
							<el-tag v-if="scope.row.status == 0"> 宕机 </el-tag>
							<el-tag v-if="scope.row.status == 1"> 工作中 </el-tag>
							<el-tag v-if="scope.row.status == 2"> 等待被唤醒 </el-tag>
						</template>
					</el-table-column>
					<el-table-column prop="description" label="描述" show-overflow-tooltip />
					<el-table-column prop="updatedTime " label="更新时间" show-overflow-tooltip />
				</el-table>
			</el-card>
		</el-drawer>
	</div>
</template>

<script setup lang="ts">
import { onMounted, reactive } from 'vue';

import { getAPI } from '/@/utils/axios-utils';
import { SysJobApi } from '/@/api-services/api';
import { SysJobCluster } from '/@/api-services/models';

const state = reactive({
	loading: false,
	isVisible: false,
	jobClusterList: [] as Array<SysJobCluster>,
});

onMounted(async () => {
	handleQuery();
});
// 查询操作
const handleQuery = async () => {
	state.loading = true;
	var res = await getAPI(SysJobApi).apiSysJobGetJobClusterListGet();
	state.jobClusterList = res.data.result ?? [];
	state.loading = false;
};
// 打开页面
const openDrawer = () => {
	state.isVisible = true;
};

// 导出
defineExpose({ openDrawer });
</script>

<style lang="scss"></style>
