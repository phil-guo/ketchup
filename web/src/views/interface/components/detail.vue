<template>
  <div class="acountAdd">
    <el-dialog title="详情" :visible="showDetail" width="40%" top="3vh" center @close="hideDialog()">
      <div class="deputyTitle">简要描述:</div>
      <div class="content">
        <span></span>
        <span>分页查询</span>
      </div>
      <div class="deputyTitle">请求URL:</div>
      <div class="content">
        <span></span>
        <span class="contentUrl">http://ip地址/api/zero/operates/PageSerachOperate</span>
      </div>
      <div class="deputyTitle">请求方式:</div>
      <div class="content">
        <span></span>
        <span>post</span>
      </div>
      <div class="deputyTitle">参数:</div>
      <el-table :data="parameterList">
        <el-table-column prop="apiUrl" label="参数" align="center"></el-table-column>
        <el-table-column prop="apiUrl" label="必选" align="center"></el-table-column>
        <el-table-column prop="apiUrl" label="类型" align="center"></el-table-column>
        <el-table-column prop="apiUrl" label="说明" align="center"></el-table-column>
      </el-table>
      <div class="deputyTitle">返回示例:</div>
      <div class="jsonData" v-html="jsonData"></div>
      <div class="deputyTitle">返回参数说明:</div>
      <el-table :data="parameterList">
        <el-table-column prop="apiUrl" label="参数名" align="center"></el-table-column>
        <el-table-column prop="apiUrl" label="类型" align="center"></el-table-column>
        <el-table-column prop="apiUrl" label="说明" align="center"></el-table-column>
      </el-table>
      <div class="deputyTitle">备注:</div>
      <div class="content">
        <span></span>
        <span>更多返回错误代码请看首页的错误代码描述</span>
      </div>
      <div slot="footer" class="dialog-footer">
        <el-button @click="cancle()">取 消</el-button>
      </div>
    </el-dialog>
  </div>
</template>
<script>
// import { getRoleList, add } from "@/api/account";
export default {
  props: {
    showDetail: {
      type: Boolean,
      default: false,
    },
  },
  data() {
    return {
      parameterList: [],
      jsonData: {},
      rules: {
        realName: [
          { required: true, message: "请输入用户名", trigger: "blur" },
        ],
        userName: [{ required: true, message: "请输入账号", trigger: "blur" }],
        roleId: [{ required: true, message: "请选择角色", trigger: "change" }],
      }, // 表单验证
    };
  },
  watch: {
    showDetail(val) {
      if (val) {
        let data = {
          msg: "ok",
          result: {
            datas: [
              {
                id: 5,
                name: "权限",
                unique: 1005,
                remark: "",
              },
            ],
            total: 5,
          },
          code: 0,
        };
        this.jsonData = this.formatJson(data);
      }
    },
  },
  methods: {
    // 查询角色列表
    // getRole() {
    //   getRoleList().then(res => {
    //     this.options = res.data;
    //   });
    // },
    // 添加
    formatJson(msg) {
      var rep = "~";
      var jsonStr = JSON.stringify(msg, null, rep);
      var str = "";
      for (var i = 0; i < jsonStr.length; i++) {
        var text2 = jsonStr.charAt(i);
        if (i > 1) {
          var text = jsonStr.charAt(i - 1);
          if (rep != text && rep == text2) {
            str += "<br/>";
          }
        }
        str += text2;
      }
      jsonStr = "";
      for (var i = 0; i < str.length; i++) {
        var text = str.charAt(i);
        if (rep == text) {
          jsonStr += "&nbsp;&nbsp;&nbsp;&nbsp;";
        } else {
          jsonStr += text;
        }
        if (i == str.length - 2) {
          jsonStr += "<br/>";
        }
      }
      return jsonStr;
    },
    confirm(val) {
      let that = this;
      this.$refs[val].validate((valid) => {
        if (valid) {
          add(this.form).then((res) => {
            this.$parent.tableloading = true;
            this.$parent.getPaginationList(that.$parent.PaginationParams);
            this.$parent.tableloading = false;
            this.hideDialog();
          });
        } else {
          return false;
        }
      });
    },
    cancle() {
      this.hideDialog();
    },
    hideDialog() {
      this.$emit("hideDetail");
    },
  },
};
</script>
<style lang="scss" scoped>
.deputyTitle {
  padding: 10px 0;
  font-size: 15px;
  color: #000;
}
.content {
  padding: 15px 0 15px 20px;
  span:nth-of-type(1) {
    display: inline-block;
    width: 5px;
    height: 5px;
    border-radius: 50%;
    background: #000;
    vertical-align: middle;
  }
  span:nth-of-type(2) {
    margin-left: 5px;
    vertical-align: middle;
  }
  .contentUrl {
    padding: 5px 10px;
    color: #d14;
    background: #f6f6f6;
    border: 1px solid #ddd;
    border-radius: 4px;
  }
}
.jsonData {
  padding: 10px;
  background: rgb(252, 252, 252);
  border: 1px solid rgb(225, 225, 232);
}
/deep/ .el-table th.is-leaf {
  background-color: #fafafa;
}
/deep/ .el-dialog--center .el-dialog__body {
  padding: 0 25px 25px 30px;
}
</style>

