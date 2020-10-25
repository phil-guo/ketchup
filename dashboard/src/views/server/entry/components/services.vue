<template>
  <div class="services">
    <div
      class="block"
      v-for="(item, index) in servicesData"
      :key="index"
      @click="toServerEntry(item)"
    >
      <span class="txt">
        {{ item }}
      </span>
      <span class="hr" v-if="index != servicesData.length - 1">/</span>
    </div>
  </div>
</template>

<script>
export default {
  name: "services",
  props: {
    scope: {
      default: null,
    },
  },
  data() {
    return {
      servicesData: [],
    };
  },
  created() {
    this.servicesData = this.scope.row.services;
  },
  methods: {
    toServerEntry(parms) {
      this.$router.push({
        path: "/serverEntry",
        query: { server: this.scope.row.name, service: parms },
      });
    },
  },
};
</script>

<style scoped>
.block {
  display: inline-block;
  padding: 6px;
  color: #2f74ff;
  border-radius: 4px;
}
.txt {
  cursor: pointer;
  vertical-align: middle;
  border-bottom: 1px solid #2f74ff;
}
.txt:hover {
  opacity: 0.8;
}
.hr {
  display: inline-block;
  padding-left: 4px;
  margin-right: -4px;
  vertical-align: middle;
  color: #606266;
}
</style>