<template>
    <b-container class='gallary'>
        <h3>Gallary</h3>
        <b-carousel
        id="carousel-1"
        :interval="4000"
        controls
        indicators
        background="#ababab"
        img-width="1024"
        img-height="480"
        style="text-shadow: 1px 1px 2px #333;">
            <b-carousel-slide
                caption="Menu screen"
                img-src="../assets/gallary-1.png"
                class='inner-img'>
            </b-carousel-slide>
            <b-carousel-slide
                caption="Player spaceship"
                img-src="../assets/gallary-2.png"
                class='inner-img'>
            </b-carousel-slide>
            <b-carousel-slide
                caption="Enemies and space"
                img-src="../assets/gallary-3.png"
                class='inner-img'>
            </b-carousel-slide>
            <b-carousel-slide
                caption="Enemy shooting"
                img-src="../assets/gallary-4.png"
                class='inner-img'>
            </b-carousel-slide>
            <b-carousel-slide
                caption="Boss enemy shooting laser"
                img-src="../assets/gallary-5.png"
                class='inner-img'>
            </b-carousel-slide>
            <b-carousel-slide
                caption="Pickup items"
                img-src="../assets/gallary-6.png"
                class='inner-img'>
            </b-carousel-slide>
            <b-carousel-slide
                caption="Interior view"
                img-src="../assets/gallary-7.png"
                class='inner-img'>
            </b-carousel-slide>
            <b-carousel-slide
                caption="Warp drive"
                img-src="../assets/gallary-8.png"
                class='inner-img'>
            </b-carousel-slide>
        </b-carousel>
        
    </b-container>
</template>

<style scoped>
    .gallary {
        margin-top: 2em;
        margin-bottom: 2em;
    }
    .inner-img {
        height: 900px;
    }
</style>

<script>
export default {
    
}
</script>