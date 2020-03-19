<template>
    <div class="hello">
        <h1>{{ msg }}</h1>
        <div>
            <iframe src="http://fundgz.1234567.com.cn/js/257070.js" style="border:0px; width:1100px; height:28px; overflow:hidden;" scrolling="no"></iframe>
            <iframe src="http://fundgz.1234567.com.cn/js/320007.js" style="border:0px; width:1024px; height:28px; overflow:hidden;" scrolling="no"></iframe>
        </div>
        <hr />
        <el-row>
            
            <el-select v-model="fundCodes" placeholder="请选择"  multiple >
                <el-option v-for="item in fundData"
                            :key="item.code"
                            :label="item.name"
                            :value="item.code" @click.native="">
                </el-option>
            </el-select>
            <el-input v-model="inputCodes" placeholder="请输入代码并用英文逗号分隔" style="margin:10px 10px;"></el-input>
            <td><el-button :inline="true" type="primary" @click="onAdd()" style="margin:0 10px;">应用</el-button></td>
            <td><el-button :inline="true" type="primary" @click="onRefresh()" style="margin:0 10px;">刷新</el-button></td>
        </el-row>
        <div>
            <img v-for="p in fundCodes" :src="getSplice(p)" :key="p" style="width:450px;height:280px;" />
        </div>
    </div>
</template>
<script>
    import request from '@/utils/request'
    export default {
        name: 'HelloWorld',
        data() {
            return {
                fundCodes: ['320007', '007301', '257070'],
                msg: 'fundList!!!',
                inputCodes: "",
                fundData:null,
            }
        },
        methods: {
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
                        _this.fundData = result.data;
                    }
                });
            },
            getSplice(code) {
                return "http://j4.dfcfw.com/charts/pic6/" + code + ".png?v=" + Math.random()
            },
            onAdd() {
                if (this.inputCodes) {
                    localStorage.fundCodes = this.inputCodes;
                    this.fundCodes = this.inputCodes.split(',');
                }
            },
            onRefresh() {
                //location.reload();
                //this.$router.go(0);
                var tempArr = this.fundCodes;
                this.fundCodes=[];
                this.fundCodes = tempArr;
            },
        },
        mounted() {
            this.getFundCodes();
            if (localStorage.fundCodes) {
                this.inputCodes = localStorage.fundCodes;
                this.fundCodes = this.inputCodes.split(',');
            } else {
                this.inputCodes = this.fundCodes.join(',');
            }
        },
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
