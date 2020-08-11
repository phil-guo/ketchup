<template>
  <div class="user-container">
    <div class="panel">
      <el-row>
        <el-form ref="form" :model="form" label-width="80px">
          <el-col :span="5">
            <el-form-item label="用户名">
              <el-input v-model="form.userName" placeholder="请输入用户名" clearable></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="5">
            <el-form-item label="账号">
              <el-input v-model="form.account" placeholder="请输入账号" clearable></el-input>
            </el-form-item>
          </el-col>
          <el-col :span="5">
            <el-form-item>
              <el-button type="primary" @click="handleSearch()">查询</el-button>
              <el-button type="success" @click="handleAdd()">添加</el-button>
            </el-form-item>
          </el-col>
        </el-form>
      </el-row>
    </div>
    <el-table :data="tableData" style="width: 100%" v-loading="isLoading">
      <el-table-column type="index" label="序号" width="100" align="center"></el-table-column>
      <el-table-column prop="userName" label="用户名" align="center"></el-table-column>
      <el-table-column prop="account" label="账号" align="center"></el-table-column>
      <el-table-column prop="roleName" label="角色" align="center"></el-table-column>
      <el-table-column prop="status" label="状态" align="center">
        <template slot-scope="scope">
          <el-switch
            v-model="scope.row.status"
            active-value="Y"
            inactive-value="N"
            @change="switchChange(scope.$index,scope.row.status)"
          ></el-switch>
        </template>
      </el-table-column>
      <el-table-column label="操作" align="center">
        <template slot-scope="scope">
          <el-button size="mini" type="success" @click="handleEdit(scope.$index, scope.row)">编辑</el-button>
          <el-button size="mini" type="danger" @click="handleDelete(scope.$index, scope.row)">删除</el-button>
        </template>
      </el-table-column>
    </el-table>
    <Pagination
      :total="page.total"
      :page.sync="page.listQuery.page"
      :limit.sync="page.listQuery.limit"
      @pagination="getList"
    />
    <AddPopups :showAdd="isAdd" v-on:hideAdd="isAdd=false" />
  </div>
</template>
<script>
import AddPopups from "@/views/system/user/components/add";
import Pagination from "@/components/pagination/pagination";
export default {
  name: "User",
  components: {
    Pagination,
    AddPopups
  },
  data() {
    return {
      isLoading: false,
      isAdd: false,
      page: {
        total: 15,
        listQuery: { page: 5, limit: 1 }
      },
      form: {},
      tableData: [
        {
          userName: "王小虎",
          account: "unter",
          roleName: "无敌管理员",
          status: "N"
        },
        {
          userName: "王小虎",
          account: "admin",
          roleName: "超级管理员",
          status: "Y"
        },
        {
          userName: "王小虎",
          account: "unter",
          roleName: "无敌管理员",
          status: "N"
        },
        {
          userName: "王小虎",
          account: "admin",
          roleName: "超级管理员",
          status: "Y"
        },
        {
          userName: "王小虎",
          account: "unter",
          roleName: "无敌管理员",
          status: "N"
        },
        {
          userName: "王小虎",
          account: "admin",
          roleName: "超级管理员",
          status: "Y"
        },
        {
          userName: "王小虎",
          account: "unter",
          roleName: "无敌管理员",
          status: "N"
        },
        {
          userName: "王小虎",
          account: "admin",
          roleName: "超级管理员",
          status: "Y"
        },
        {
          userName: "王小虎",
          account: "unter",
          roleName: "无敌管理员",
          status: "N"
        },
        {
          userName: "王小虎",
          account: "unter",
          roleName: "无敌管理员",
          status: "N"
        }
      ]
    };
  },
  methods: {
    //获取分页
    getList() {
      this.tableData.push({
        userName: "王小虎",
        account: "admin",
        roleName: "超级管理员",
        status: "Y"
      });
    },
    //查询按钮
    handleSearch() {
      this.isLoading = true;
    },
    //添加弹窗
    handleAdd() {
      this.isAdd = true;
    },
    //监听表格switch
    switchChange(index, value) {
      this.tableData[index].status = value;
    },
    handleEdit() {},
    handleDelete() {}
  }
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
