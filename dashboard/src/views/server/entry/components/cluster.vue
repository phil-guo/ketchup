<template>
  <div>
    <div class="table">
      <div class="tableHeader">
        <div>key</div>
        <div>ip</div>
        <div>端口</div>
      </div>
      <div class="tableContent">
        <div
          class="tableBody"
          v-for="(item, index) in clusterData"
          :key="index"
        >
          <div>{{ item.key }}</div>
          <div>{{ item.ip }}</div>
          <div>{{ item.port }}</div>
        </div>
      </div>
    </div>

    <!-- <el-table :data="clusterData">
      <el-table-column prop="key" label="key"> </el-table-column>
      <el-table-column prop="ip" label="ip"> </el-table-column>
      <el-table-column prop="port" label="端口"> </el-table-column>
    </el-table> -->
  </div>
</template>

<script>
export default {
  props: {
    scope: {
      default: null,
    },
  },
  data() {
    return {
      clusterData: [],
    };
  },
  created() {
    this.clusterData = this.scope.row.cluster;
  },
  methods: {},
};
</script>

<style scoped>
.table {
  background: #f5f7fa;
}
.tableHeader {
  display: flex;
  padding: 6px 0 6px 6px;
  font-size: 14px;
  border-bottom: 1px solid #eee;
}
.tableHeader div {
  width: 33.3%;
}
.tableBody {
  display: flex;
  padding: 6px 0 6px 6px;
  font-size: 14px;
  border-bottom: 1px solid #eee;
}
.tableBody div {
  width: 33.3%;
}
.tableContent .tableBody:last-child {
  border: none;
}
</style>