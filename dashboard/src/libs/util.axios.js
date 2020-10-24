/**
 * http 请求工具包
 */

import cookies from './util.cookies'
import globalSetting from './util.setting'
import axios from 'axios'

const http = {}

/**
 * @description post请求
 * @param {json} data 请求参数
 * @param {String} url 请求路径
 * @param {object} vm 当前上下文
 * @param {String} callback 回掉函数
 */
http.post = function (url, data, vm, callback) {
  axios
    .post(url, data, {
      headers: {
        Authorization: cookies.get(globalSetting.token)
      }
    })
    .then(response => {
      if (response.data.code == 0) {
        callback(response.data.result)
      } else {
        vm.$notify.error({
          title: globalSetting.operateErrorMsg,
          message: response.data.msg
        })
      }
    })
    .catch(err => {
      if (err.response.code == 401) {
        vm.$router.push({
          name: 'login'
        })
      } else if (err.response.code == 403) {
        vm.$message({
          type: 'error',
          message: globalSetting.tokenMsg
        })
      }
    })
}

http.get = function (url, vm, callback) {
  axios
    .get(
      url, {}, {
        headers: {
          Authorization: cookies.get(globalSetting.token)
        }
      }
    )
    .then(response => {
      if (response.status == 1) {
        callback(response)
      } else {
        vm.$notify.error({
          title: globalSetting.operateErrorMsg,
          message: response.msg
        })
      }
    })
    .catch(err => {
      if (err.response.status == 401) {
        vm.$router.push({
          name: 'login'
        })
      } else if (err.response.status == 403) {
        vm.$message({
          type: 'error',
          message: globalSetting.tokenMsg
        })
      }
    })
}

export default http
