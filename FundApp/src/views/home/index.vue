<template>
    <div class="hello">
        <h1>{{ msg }}</h1>
        <hr />
        <h2>CORE_ENV：{{netcoreEnv}}</h2>
        <h2>NODE_ENV：{{nodeEnv}}</h2>
    </div>
</template>
<script>
    import request from '@/utils/request'
    export default {
        name: 'HelloWorld',
        data() {
            return {
                netcoreEnv: "",
                nodeEnv:"",
                msg: 'Welcome FundApp!!!',
            }
        },
        methods: {
            getEnv() {
                var _this = this;
                request({
                    url: "/Home/GetEnvironmentVariable",
                }).then(result => {
                    console.log(result)
                    if (result) {
                        _this.netcoreEnv = result.data;
                    }
                });
            },
        },
        created() {
            this.nodeEnv = process.env.NODE_ENV;
            console.log(process.env.NODE_ENV)
        },
        mounted() {
            this.getEnv();   
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
