<template>
    <div>
        <el-row element-loading-text="拼命加载中..."
                element-loading-spinner="el-icon-loading"
                element-loading-background="rgba(0, 0, 0, 0.8)"
                v-loading.fullscreen.lock="fullscreenLoading">

            <el-row>
                <el-col :span="6" class="col1">
                    <el-select v-model="fundCode" placeholder="请选择" @change="slChange" multiple style="width:310px;">
                        <el-option v-for="item in fundCodes"
                                   :key="item.code"
                                   :label="item.name"
                                   :value="item.code" @click.native="optionClick(item.code)">
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
                                        :picker-options="pickerOptions" @change="">
                        </el-date-picker>
                    </div>
                </el-col>
                <el-col :span="12" class="col1">
                    <el-button @click="search">查询</el-button>
                </el-col>

            </el-row>
            <el-row>
                <el-tabs v-model="activeName" @tab-click="">
                    <div id="dailyChart" style="width: 100%;height:220px;"></div>
                    <el-tab-pane label="" name="chart">
                        <div v-for="(item) in fundCode" :key="item" :id="prefix(item)" style="width: 100%;height:220px;">
                        </div>
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
        name: 'dayAllChart',
        data() {
            return {
                msg: 'fundList!!!',
                fullscreenLoading: false,
                checkAllCode:'000000',
                dayChart: null,
                dateRanges: '',
                fundCode: [],
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
                colors: ['#409EFF', '#67C23A', '#E6A23C', '#F56C6C', '#7FFFAA', '#FFD700', '#00FF00']
            }
        },
        mounted() {
            this.dayChart = this.$echarts.init(document.getElementById('dailyChart'));
            this.getFundCodes();
        },
        methods: {
            search: function (selVal) {
                if (this.fundCode.length == 0) {
                    this.$alert('要选择代码啊，大兄弟！！！', '提示', {
                        confirmButtonText: '确定',
                        callback: action => {

                        }
                    });
                    return false;
                }
                this.fullscreenLoading = true;
                this.getChartData(this.fundCode);
            },
            prefix(suffix) {
                return 'cahrt_' + suffix;
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
                        result.data.unshift({ code: _this.checkAllCode, name: '---全选---' });
                    }
                });
            },
            optionClick(option) {
                if (option == this.checkAllCode) {
                    if (this.fundCode.length > 0) {
                        this.fundCode = [];
                    } else {
                        const allValues = [];
                        this.fundCodes.forEach(item => {
                            if (item.code != this.checkAllCode) {
                                allValues.push(item.code);
                            }
                        });
                        this.fundCode = allValues;
                    }
                }

            },
            slChange(selectVal) {
                let code = this.checkAllCode;
                var pindex = this.fundCode.findIndex(function (item) {
                    return item == code;
                });
                if (pindex >= 0) {
                    this.fundCode.splice(pindex, 1);
                }
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
                    url: "/Home/GetAllChartData",
                    methods: "get",
                    params: { fundCodes: this.fundCode.join(','), date1, date2 },
                }).then(response => {
                    _this.fullscreenLoading = false;
                    var result = response.data;
                    if (result) {
                        var cindex = 0;
                        _this.fundCode.forEach(function (code) {
                            var sdata = result.listY.filter(x => x.code == code);
                            var elChart = _this.$echarts.init(document.getElementById('cahrt_' + code));
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
                                legend: {
                                    data: legends
                                },
                                title: {
                                    left: 'center',
                                    //text: _this.fundCode,
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
                                series: {
                                    name: '[' + sdata[0].name + '(' + sdata[0].code + ')]累计净值增长率',
                                    type: 'line',
                                    showSymbol: false,
                                    itemStyle: {
                                        color: _this.colors[cindex]
                                    },
                                    data: sdata[0].y
                                }
                            };
                            elChart.setOption(chartOptions);
                            cindex++;
                        });

                        cindex = 0;
                        var series = [];
                        var legends = [];
                        result.listY.forEach(function (item) {
                            series.push({
                                name: '[' + item.code + ']累计净值增长率',
                                type: 'line',
                                //showSymbol: false,
                                itemStyle: {
                                    color: _this.colors[cindex]
                                },
                                data: item.y
                            });
                            legends.push(item.name + '(' + item.code + ')');
                            cindex++;
                            if (cindex > _this.colors.length) {
                                cindex = parseInt(Math.random() * (_this.colors.length + 1));
                            }
                        });
                        //累计
                        var chartAllOptions = {
                            tooltip: {
                                trigger: 'axis',
                                position: function (pt) {
                                    return [pt[0], '10%'];
                                },
                                //formatter: function (s) {
                                //    return s[0].seriesName + "<br/>" + dateFormat(,new Date(s[0].axisValue),'yyyy-MM-dd') + "<br/>" + s[0].value + "%";
                                //},
                            },
                            legend: {
                                data: legends
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
                            series: series
                        };
                        _this.dayChart.setOption(chartAllOptions);
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
