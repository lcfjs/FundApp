import Vue from 'vue'
import Router from 'vue-router'
//import HelloWorld from '@/components/HelloWorld'
import Layout from '@/layout'

Vue.use(Router)

export default new Router({
    routes: [
        {
            path: '/',
            name: 'dashboard',
            component: Layout,
            redirect: '/home',
            children: [
                {
                    path: 'home',
                    name: 'home',
                    component: () => import('@/views/home/index'),
                    meta: { title: 'Home', icon:'dashboard' }
                }
            ]
        },
        {
            path: '/fundList',
            component: Layout,
            children: [
                {
                    path: 'list',
                    name: 'fundList-list',
                    component: () => import('@/views/fundList/index'),
                    meta: { title: 'FundList', icon: 'form' }
                }
            ]
        },
        {
            path: '/dayChart',
            component: Layout,
            children: [
                {
                    path: 'index',
                    name: 'dayChart-index',
                    component: () => import('@/views/dayChart/index'),
                    meta: { title: 'DayChart', icon: 'chart' }
                }
            ]
        },
        {
            path: '/dayAllChart',
            component: Layout,
            children: [
                {
                    path: 'index',
                    name: 'dayAllChart-index',
                    component: () => import('@/views/dayAllChart/index'),
                    meta: { title: 'DayAllChart', icon: 'drag' }
                }
            ]
        },
        {
            path: '/minChart',
            component: Layout,
            children: [
                {
                    path: 'index',
                    name: 'minChart-index',
                    component: () => import('@/views/minChart/index'),
                    meta: { title: 'MinChart', icon: 'international' }
                }
            ]
        },
    ]
})
