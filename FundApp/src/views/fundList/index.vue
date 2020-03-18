<template>
    <div class="hello">
        <h1>{{ msg }}</h1>
        <div>
            <iframe src="http://fundgz.1234567.com.cn/js/257070.js" style="border:0px; width:1100px; height:28px; overflow:hidden;" scrolling="no"></iframe>
            <iframe src="http://fundgz.1234567.com.cn/js/320007.js" style="border:0px; width:1024px; height:28px; overflow:hidden;" scrolling="no"></iframe>
        </div>
        <hr />
        <div>
            <img v-for="p in fundCodes" :src="getSplice(p)" :key="p" style="width:450px;height:280px;"/>
        </div>
    </div>
</template>
<script>
    export default {
        name: 'HelloWorld',
        data() {
            return {
                fundCodes: ['320007', '007301','257070'],
                msg: 'fundList!!!',

            }
        },
        methods: {
            getSplice(code) {
                return "http://j4.dfcfw.com/charts/pic6/" + code + ".png?v=" + Math.random()
            },
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
