<template>
    <div class="hello">
        <h1>{{ msg }}</h1>
        <h2>Essential Links</h2>
    </div>
</template>
<script>
    export default {
        name: 'HelloWorld',
        data() {
            return {
                msg: 'Welcome FundApp!!!'
            }
        },
        created() {
            console.log(process.env.NODE_ENV)
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
