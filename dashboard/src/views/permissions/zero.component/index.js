import zeroComponent from './index.vue'

const zeroComponent = {
    install: function (Vue) {
        Vue.component('zero-permission', zeroComponent)
    }
}

//导出组件
export default zeroComponent