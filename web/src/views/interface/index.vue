<template>
  <div class="user-container">
    <div class="panel">
      <el-row>
        <el-form ref="page" :model="page" label-width="80px">
          <el-col :span="5">
            <el-form-item label="接口名称">
              <el-input v-model="page.userName" placeholder="请输入接口名称" clearable></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="5">
            <el-form-item label-width="20px">
              <el-button type="primary" @click="handleSearch()" icon="el-icon-search">查询</el-button>
              <!-- <el-button type="success" @click="handleAdd()" icon="el-icon-plus">添加</el-button> -->
            </el-form-item>
          </el-col>
        </el-form>
      </el-row>
    </div>
    <el-table :data="tableData" style="width: 100%" v-loading="isLoading">
      <el-table-column type="index" label="序号" width="100" align="center"></el-table-column>
      <el-table-column prop="apiUrl" label="xxx" align="center"></el-table-column>
      <el-table-column prop="apiUrl" label="接口描述" align="center"></el-table-column>
      <el-table-column label="操作" align="center" width="200">
        <template slot-scope="scope">
          <el-button size="mini" type="success" @click="handleDetail(scope.$index, scope.row)">详情</el-button>
        </template>
      </el-table-column>
    </el-table>
    <Pagination
      :total="page.total"
      :page.sync="page.page"
      :limit.sync="page.limit"
      @pagination="getList"
    />
    <DetailPopups :showDetail="isDetail" v-on:hideDetail="isDetail=false" />
  </div>
</template>
<script>
import DetailPopups from "@/views/interface/components/detail";
import Pagination from "@/components/pagination/pagination";
export default {
  name: "User",
  components: {
    Pagination,
    DetailPopups,
  },
  data() {
    return {
      isLoading: false,
      isDetail: false,
      page: {
        total: 2,
        page: 1,
        limit: 10,
      },
      tableData: [
        { apiUrl: "www/gg/user", apiName: "用户管理列表" },
        { apiUrl: "www/gg/login", apiName: "登陆" },
      ],
    };
  },
  methods: {
    //获取分页
    getList() {},
    //查询按钮
    handleSearch() {
      this.isLoading = true;
    },
    //添加弹窗
    handleAdd() {},
    //详情弹窗
    handleDetail() {
      this.isDetail = true;
    },
    handleDelete() {},
  },
};
</script>
<style lang="scss" scoped>
.panel {
  margin-bottom: 20px;
  padding: 30px 0;
  background-color: #fff;
  border: 1px solid #eee;
  box-shadow: 1px 5px 1px #eee;
}
.el-form-item {
  margin: 0;
}
</style>
