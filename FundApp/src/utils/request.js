import axios from 'axios'
import { MessageBox, Message } from 'element-ui'
//import store from '@/store'
import { getToken } from '@/utils/auth'

/**
 *  `params` 是即将与请求一起发送的 URL 参数  params: {ID: 12345  },
 *  `data` 是作为请求主体被发送的数据,只适用于这些请求方法 'PUT', 'POST', 和 'PATCH'  data: {firstName: 'Fred'  },
 *  `auth` 表示应该使用 HTTP 基础验证，并提供凭据
 *  `headers` 是即将被发送的自定义请求头
 */

let base_url = null;
if ('development' == process.env.NODE_ENV) base_url = 'http://localhost:55161';

// create an axios instance
const service = axios.create({
    baseURL: base_url,//process.env.VUE_APP_BASE_API, // url = base url + request url
    // withCredentials: true, // send cookies when cross-domain requests
    timeout: 10000 // request timeout
})

// request interceptor
service.interceptors.request.use(
    config => {
        // do something before request is sent

        //if (store.getters.token) {
        //  // let each request carry token
        //  // ['X-Token'] is a custom headers key
        //  // please modify it according to the actual situation
        //  config.headers['X-Token'] = getToken()
        //}
        return config
    },
    error => {
        // do something with request error
        console.log(error) // for debug
        return Promise.reject(error)
    }
)

// response interceptor
service.interceptors.response.use(
    /**
     * If you want to get http information such as headers or status
     * Please return  response => response
    */

    /**
     * Determine the request status by custom code
     * Here is just an example
     * You can also judge the status by HTTP Status Code
     */
    response => {
        const res = response.data

        // if the custom code is not 20000, it is judged as an error.
        //if (res.code !== 20000) {
        //  Message({
        //    message: res.message || 'Error',
        //    type: 'error',
        //    duration: 5 * 1000
        //  })

        //  // 50008: Illegal token; 50012: Other clients logged in; 50014: Token expired;
        //  if (res.code === 50008 || res.code === 50012 || res.code === 50014) {
        //    // to re-login
        //    MessageBox.confirm('You have been logged out, you can cancel to stay on this page, or log in again', 'Confirm logout', {
        //      confirmButtonText: 'Re-Login',
        //      cancelButtonText: 'Cancel',
        //      type: 'warning'
        //    }).then(() => {
        //      store.dispatch('user/resetToken').then(() => {
        //        location.reload()
        //      })
        //    })
        //  }
        //  return Promise.reject(new Error(res.message || 'Error'))
        //} else {
        //  return res
        //}
        if (res.code == 0) {

        } else {
            Message({
                message: res.msg || 'Error',
                type: 'error',
                duration: 3 * 1000
            })
        }

        return res;
    },
    error => {
        console.log('err' + error) // for debug
        Message({
            message: error.message,
            type: 'error',
            duration: 5 * 1000
        })
        return Promise.reject(error)
    }
)

export default service
