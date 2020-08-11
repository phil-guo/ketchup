<template>
  <div class="acountAdd">
    <el-dialog title="新增" :visible="showAdd" top="20vh" @close="hideData()">
      <el-form ref="form" :model="form" :rules="rules">
        <el-form-item label="用户名" :label-width="formLabelWidth" prop="realName">
          <el-input v-model="form.realName" auto-complete="off" placeholder="请输入用户名" clearable />
        </el-form-item>
        <el-form-item label="账号" :label-width="formLabelWidth" prop="userName">
          <el-input v-model="form.userName" auto-complete="off" placeholder="请输入账号" clearable />
        </el-form-item>
        <el-form-item label="角色" :label-width="formLabelWidth" prop="roleId">
          <el-select v-model="form.roleId" placeholder="请选择角色">
            <el-option
              v-for="item in options"
              :key="item.id"
              :label="item.text"
              :value="item.value"
            />
          </el-select>
        </el-form-item>
        <el-form-item label="状态" :label-width="formLabelWidth">
          <el-switch
            v-model="form.status"
            active-color="rgba(0,153,255,1)"
            inactive-color="#eee"
            active-value="Y"
            inactive-value="N"
          />
        </el-form-item>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button @click="cancle()">取 消</el-button>
        <el-button type="primary" @click="confirm('form')">确 定</el-button>
      </div>
    </el-dialog>
  </div>
</template>
<script>
// import { getRoleList, add } from "@/api/account";
export default {
  props: {
    showAdd: {
      type: Boolean,
      default: false,
    },
  },
  data() {
    return {
      form: {
        realName: "",
        userName: "",
        roleId: "",
        status: "Y",
      }, // 表单
      options: [
        { text: "超级管理员", value: "1" },
        { text: "无敌管理员", value: "2" },
      ],
      rules: {
        realName: [
          { required: true, message: "请输入用户名", trigger: "blur" },
        ],
        userName: [{ required: true, message: "请输入账号", trigger: "blur" }],
        roleId: [{ required: true, message: "请选择角色", trigger: "change" }],
      }, // 表单验证
      formLabelWidth: "120px",
    };
  },
  //   watch: {
  //     showAdd: "getRole"
  //   },
  methods: {
    // 查询角色列表
    // getRole() {
    //   getRoleList().then(res => {
    //     this.options = res.data;
    //   });
    // },
    // 添加
    confirm(val) {
      let that = this;
      this.$refs[val].validate((valid) => {
        if (valid) {
          this.$confirm("是否确认添加账号？", "添加账号", {
            confirmButtonText: "确定",
            cancelButtonText: "取消",
            type: "warning",
          }).then(() => {
            add(this.form).then((res) => {
              this.$parent.tableloading = true;
              setTimeout(function () {
                that.$parent.tableloading = false;
                that.$parent.getPaginationList(that.$parent.PaginationParams);
              }, 1200);
              this.hideData();
            });
          });
        } else {
          return false;
        }
      });
    },
    cancle() {
      this.hideData();
    },
    hideData() {
      this.$emit("hideAdd");
      this.$refs.form.resetFields();
    },
  },
};
</script>
<style lang="scss" scoped>
</style>

