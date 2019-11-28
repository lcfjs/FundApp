<template>
    <div element-loading-text="拼命加载中..."
         element-loading-spinner="el-icon-loading"
         element-loading-background="rgba(0, 0, 0, 0.8)"
         v-loading.fullscreen.lock="fullscreenLoading">
        <el-row style="margin:10px 20px;">
            <el-button @click="refreshData">刷新</el-button>
        </el-row>
        <el-row>
            <div id="minChart" style="width: 100%;height:400px;"></div>
        </el-row>
    </div>
</template>
<script>
    import { pageSize, pageIndex, parseTime, dateFormat } from '@/utils/common'
    import request from '@/utils/request'

    export default {
        name: 'dayChart',
        data() {
            return {
                fullscreenLoading: false,
                minChart: null,
                dateRanges: '',
                fundCode: '257070',
            }
        },
        mounted() {
            this.minChart = this.$echarts.init(document.getElementById('minChart'));
            this.refreshData();
        },
        methods: {
            refreshData: function (selVal) {
                this.fullscreenLoading = true;
                this.getChartData(this.fundCode);
            },
            getFundCodes: function () {
                var _this = this;
                request({
                    url: "/Home/GetGatherList",
                    params: null
                }).then(result => {
                    if (result) {
                        result.data.forEach(function (x) {
                            x.name = x.name + '(' + x.code + ')'
                        });
                        _this.fundCodes = result.data;
                        _this.refreshData();
                    }
                });
            },
            getChartData: function () {
                var _this = this;
                request({
                    url: "/Home/GetMinChartData",
                    methods: "get",
                    params: { fundCode: this.fundCode },
                }).then(response => {
                    _this.fullscreenLoading = false;
                    var result = response.data;
                    if (result) {
                        var chartOptions = {
                            tooltip: {
                                trigger: 'axis',
                                position: function (pt) {
                                    return [pt[0], '10%'];
                                }
                            },
                            title: {
                                left: 'center',
                                text: _this.fundCode,
                            },
                            toolbox: {
                                //feature: {
                                //    dataZoom: {
                                //        yAxisIndex: 'none'
                                //    },
                                //    restore: {},
                                //    saveAsImage: {}
                                //}
                            },
                            xAxis: {
                                type: 'category',
                                boundaryGap: false,
                                data: result.x
                            },
                            yAxis: {
                                type: 'value',
                                boundaryGap: [0, '100%']
                            },
                            series: [
                                {
                                    name: '估算收益（%）',
                                    type: 'line',
                                    smooth: true,
                                    symbol: 'none',
                                    sampling: 'average',
                                    itemStyle: {
                                        color: 'rgb(255, 70, 131)'
                                    },
                                    areaStyle: {
                                        color: new _this.$echarts.graphic.LinearGradient(0, 0, 0, 1, [{
                                            offset: 0,
                                            color: 'rgb(255, 158, 68)'
                                        }, {
                                            offset: 1,
                                            color: 'rgb(255, 70, 131)'
                                        }])
                                    },
                                    data: result.y2
                                }
                            ]
                        };
                        _this.minChart.setOption(chartOptions);
                        
                    }
                }).catch(function (error) {
                    _this.fullscreenLoading = false;
                    console.log(error);
                })
            }
        }
    }
</script>
<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>

    h1, h2 {
        font-weight: normal;
    }

    a {
        color: #42b983;
    }
</style>
