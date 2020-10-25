<template>
  <d2-container>
    <d2-crud
      ref="d2Crud"
      :columns="columns"
      :data="data"
      :rowHandle="rowHandle"
      @operate="handleSearch"
    >
    </d2-crud>
    <!--查看Json参数弹窗-->
    <el-dialog :title="title" :visible.sync="dialogFormVisible" width="600px">
      <pre >{{ demo }}</pre>

      <div slot="footer" class="dialog-footer">
        <el-button @click="dialogFormVisible = false">取 消</el-button>
      </div>
    </el-dialog>
  </d2-container>
</template>

<script>
import util from "@/libs/util.js";
export default {
  data() {
    return {
      title: "Json参数",
      dialogFormVisible: false,
      columns: [
        {
          title: "服务",
          key: "serviceName",
        },
        {
          title: "服务地址",
          key: "apiUrl",
        },
        {
          title: "描述",
          key: "description",
        },
      ],
      data: [],
      rowHandle: {
        custom: [
          {
            text: "参数",
            type: "warning",
            size: "small",
            emit: "operate",
          },
        ],
      },
      demo:
        '{"msg":"ok","result":[{"serviceName":"menus","apiUrl":"api/zero/menus/createOrEditMenu","description":null,"parameter":null,"clientType":"Ketchup.Permission.RpcMenu+RpcMenuClient"},{"serviceName":"menus","apiUrl":"api/zero/menus/getMenusByRole","description":null,"parameter":null,"clientType":"Ketchup.Permission.RpcMenu+RpcMenuClient"},{"serviceName":"menus","apiUrl":"api/zero/menus/getMenusSetRole","description":null,"parameter":null,"clientType":"Ketchup.Permission.RpcMenu+RpcMenuClient"},{"serviceName":"menus","apiUrl":"api/zero/menus/pageSerachMenu","description":null,"parameter":null,"clientType":"Ketchup.Permission.RpcMenu+RpcMenuClient"},{"serviceName":"menus","apiUrl":"api/zero/menus/removeMenu","description":null,"parameter":null,"clientType":"Ketchup.Permission.RpcMenu+RpcMenuClient"}],"code":0}',
    };
  },
  created() {
    this.getData();
  },
  mounted() {
    this.demo = JSON.parse(this.dealJson(this.demo), undefined, 4);
  },

  methods: {
    getData() {
      let vm = this;
      util.http.get(
        util.requestUrl.getAllServerEntry +
          "?server=" +
          this.$route.query.server +
          "&service=" +
          this.$route.query.service,
        vm,
        function (response) {
          vm.data = response;
        }
      );
    },

    handleSearch({ index, row }) {
      this.dialogFormVisible = true;
      return;
      console.log(row.parameter);
    },

    dealJson(json) {
      //   document.body.appendChild(document.createElement("pre")).innerHTML = inp;
      json = json.replace(/&/g, "&").replace(/>/g, ">");
      return json.replace(
        /("(\\u[a-zA-Z0-9]{4}|\\[^u]|[^\\"])*"(\s*:)?|\b(true|false|null)\b|-?\d+(?:\.\d*)?(?:[eE][+\-]?\d+)?)/g,
        function (match) {
          var cls = "number";
          if (/^"/.test(match)) {
            if (/:$/.test(match)) {
              cls = "key";
            } else {
              cls = "string";
            }
          } else if (/true|false/.test(match)) {
            cls = "boolean";
          } else if (/null/.test(match)) {
            cls = "null";
          }
          return "" + match + "";
        }
      );
    },
  },
};
</script>

<style scoped>
</style>