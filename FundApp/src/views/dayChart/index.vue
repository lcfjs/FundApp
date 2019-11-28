<template>
    <div element-loading-text="拼命加载中..."
         element-loading-spinner="el-icon-loading"
         element-loading-background="rgba(0, 0, 0, 0.8)"
         v-loading.fullscreen.lock="fullscreenLoading">
        <el-row>
            <el-row style="margin:10px 5px;">
                <el-col :span="6" class="col1">
                    <el-select v-model="fundCode" placeholder="请选择" @change="refreshData" style="width:310px;">
                        <el-option v-for="item in fundCodes"
                                   :key="item.code"
                                   :label="item.name"
                                   :value="item.code">
                        </el-option>
                    </el-select>
                </el-col>
                <el-col :span="6" class="col1">
                    <div class="block">
                        <el-date-picker v-model="dateRanges"
                                        type="daterange"
                                        align="right"
                                        unlink-panels
                                        range-separator="至"
                                        start-placeholder="开始日期"
                                        end-placeholder="结束日期"
                                        format="yyyy-MM-dd"
                                        :picker-options="pickerOptions" @change="refreshData">
                        </el-date-picker>
                    </div>
                </el-col>
                <el-col :span="12" class="col1">
                    <el-input v-model="addCode" placeholder="请输入代码" style="width:120px;"></el-input>
                    <el-button @click="insert">添加采集</el-button>
                </el-col>

            </el-row>
            <el-row>
                <el-tabs v-model="activeName" @tab-click="">
                    <el-tab-pane label="每日收益图表" name="chart">
                        <div id="dailyChart" style="width: 100%;height:600px;"></div>
                    </el-tab-pane>
                    <el-tab-pane label="累计收益图表" name="accumulativeChart">
                        <div id="accumulativeChart" style="width: 100%;height:600px;"></div>
                    </el-tab-pane>
                    <el-tab-pane label="数据列表" name="table">
                        <el-container>
                            <el-main>
                                <el-table :data="tableData" stripe border
                                          style="width: 100%">
                                    <el-table-column prop="jzrq"
                                                     label="日期"
                                                     width="180">
                                        <template slot-scope="scope">
                                            {{ dateFormat(new Date(scope.row['jzrq']),'yyyy-MM-dd') }}
                                        </template>
                                    </el-table-column>
                                    <el-table-column prop="jz"
                                                     label="净值"
                                                     width="200">
                                    </el-table-column>
                                    <el-table-column prop="ljjz"
                                                     label="累计净值"
                                                     width="200">
                                    </el-table-column>
                                    <el-table-column prop="jzzzl"
                                                     label="净值增长率">
                                        <template slot-scope="scope">
                                            {{ scope.row['jzzzl'] }}%
                                        </template>
                                    </el-table-column>
                                </el-table>
                            </el-main>
                            <el-footer>
                                <el-pagination background
                                               layout="prev, pager, next"
                                               :total="total" :current-page.sync="pageIndex" :page-size.sync="pageSize" @size-change="getTableData" @current-change="getTableData">
                                </el-pagination>
                            </el-footer>
                        </el-container>

                    </el-tab-pane>
                </el-tabs>
            </el-row>

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
                dailyChart: null,
                accumulativeChart: null,
                dateRanges: '',
                fundCode: '257070',
                addCode: "",
                tableData: [],
                activeName: "chart",
                fundCodes: [],
                pageIndex: pageIndex,
                pageSize: pageSize,
                total: 0,
                pickerOptions: {
                    shortcuts: [{
                        text: '最近一周',
                        onClick(picker) {
                            const end = new Date();
                            const start = new Date();
                            start.setTime(start.getTime() - 3600 * 1000 * 24 * 7);
                            picker.$emit('pick', [start, end]);
                        }
                    }, {
                        text: '最近一个月',
                        onClick(picker) {
                            const end = new Date();
                            const start = new Date();
                            start.setTime(start.getTime() - 3600 * 1000 * 24 * 30);
                            picker.$emit('pick', [start, end]);
                        }
                    }, {
                        text: '最近三个月',
                        onClick(picker) {
                            const end = new Date();
                            const start = new Date();
                            start.setTime(start.getTime() - 3600 * 1000 * 24 * 90);
                            picker.$emit('pick', [start, end]);
                        }
                    }]
                },
            }
        },
        mounted() {
            this.dailyChart = this.$echarts.init(document.getElementById('dailyChart'));
            let elAccumulativeChart = document.getElementById('accumulativeChart');
            //elAccumulativeChart.style.width = window.innerWidth - 20 + 'px';
            elAccumulativeChart.style.width = document.getElementById("dailyChart").offsetWidth+ 'px';
            this.accumulativeChart = this.$echarts.init(elAccumulativeChart);
            this.getFundCodes();
        },
        methods: {
            insert: function () {
                var _this = this;
                _this.fullscreenLoading = true;
                request({
                    url: "/Home/AddGather",
                    params: { code: _this.addCode }
                }).then(result => {
                    _this.fullscreenLoading = false;
                    if (result) {
                        _this.$message({
                            message: result.msg,
                            type: 'warning'
                        });
                    }
                });
            },
            dateFormat(d) {
                return dateFormat(d)
            },
            refreshData: function (selVal) {
                this.fullscreenLoading = true;
                this.getTableData(this.fundCode);
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
            getTableData: function () {
                var _this = this;
                request({
                    url: "/Home/GetDayTableData",
                    params: { fundCode: _this.fundCode, pageIndex: _this.pageIndex - 1, pageSize: _this.pageSize }
                }).then(response => {
                    var result = response.data;
                    if (result) {
                        _this.tableData = result.data;
                        _this.total = result.total;
                    }
                });
            },
            getChartData: function () {
                var _this = this;
                var date1 = null;
                var date2 = null;
                if (this.dateRanges) {
                    date1 = dateFormat(new Date(this.dateRanges[0]), 'yyyy-MM-dd')
                    date2 = dateFormat(new Date(this.dateRanges[1]), 'yyyy-MM-dd')
                }
                request({
                    url: "/Home/GetDayChartData",
                    methods: "get",
                    params: { fundCode: this.fundCode, date1, date2 },
                }).then(response => {
                    _this.fullscreenLoading = false;
                    var result = response.data;
                    if (result) {
                        var chartOptions = {
                            tooltip: {
                                trigger: 'axis',
                                position: function (pt) {
                                    return [pt[0], '10%'];
                                },
                                formatter: function (s) {
                                    return s[0].seriesName + "<br/>" + dateFormat(new Date(s[0].axisValue), 'yyyy-MM-dd') + "<br/>" + s[0].value + "%";
                                },
                            },
                            title: {
                                left: 'center',
                                text: _this.fundCode,
                            },
                            xAxis: {
                                type: 'category',
                                boundaryGap: false,
                                axisLabel: {
                                    formatter: function (value) {
                                        return dateFormat(new Date(value), 'yyyy-MM-dd');
                                    }
                                },
                                data: result.x,
                            },
                            yAxis: {
                                type: 'value',
                            },
                            series: [
                                {
                                    name: '每日净值增长率',
                                    type: 'bar',
                                    barWidth: 20, //柱子宽度
                                    showSymbol: false,
                                    itemStyle: {
                                        normal: {
                                            color: function (arg) {
                                                if (arg.value > 0) {
                                                    return '#F56C6C';
                                                } else {
                                                    return '#67C23A';
                                                }
                                            },
                                            label: {
                                                show: true, //开启显示
                                                position: 'top', //在上方显示
                                                //textStyle: { //数值样式
                                                //    color: 'black',
                                                //    fontSize: 16
                                                //}
                                            }
                                        }
                                    },
                                    data: result.y
                                }
                            ]
                        };
                        _this.dailyChart.setOption(chartOptions);

                        //累计
                        var accumulativeOptions = {
                            tooltip: {
                                trigger: 'axis',
                                position: function (pt) {
                                    return [pt[0], '10%'];
                                },
                                formatter: function (s) {
                                    return s[0].seriesName + "<br/>" + dateFormat(new Date(s[0].axisValue), 'yyyy-MM-dd') + "<br/>" + s[0].value + "%";
                                },
                            },
                            title: {
                                left: 'center',
                                text: _this.fundCode,
                            },
                            xAxis: {
                                type: 'category',
                                boundaryGap: false,
                                axisLabel: {
                                    formatter: function (value) {
                                        return dateFormat(new Date(value), 'yyyy-MM-dd');
                                    }
                                },
                                data: result.x,
                            },
                            yAxis: {
                                type: 'value',
                            },
                            series: [
                                {
                                    name: '累计净值增长率',
                                    type: 'line',
                                    showSymbol: false,
                                    itemStyle: {
                                        color: 'rgb(255, 70, 131)'
                                    },
                                    data: result.y2
                                }
                            ]
                        };
                        _this.accumulativeChart.setOption(accumulativeOptions);
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
