$(document).ready(function () {
    //Ref : https://www.amcharts.com/demos/tag-cloud/#code

    var url = $('#tagCloudUrl').val();
    var searchUrl = $('#tagCloudSearchUrl').val();
    function tagCloidInit() {
        $.ajax({
            url: url,
            method: 'GET',
            dataType: 'json',
            success: function (res) {
                generateTagCloug(res, searchUrl);
                //console.log(res);
            },
            error: function (err) { console.log(err) },
        });
    }

    function generateTagCloug(data, surl) {
        am4core.ready(function () {
            // Themes begin
            am4core.useTheme(am4themes_animated);
            // Themes end

            var chart = am4core.create("chartdiv", am4plugins_wordCloud.WordCloud);
            chart.fontFamily = "Courier New";
            var series = chart.series.push(new am4plugins_wordCloud.WordCloudSeries());
            series.randomness = 0.1;
            series.rotationThreshold = 0.5;

            series.data = data;

            series.dataFields.word = "tag";
            series.dataFields.value = "count";

            series.heatRules.push({
                "target": series.labels.template,
                "property": "fill",
                "min": am4core.color("#0000CC"),
                "max": am4core.color("#CC00CC"),
                "dataField": "value"
            });
            //https://localhost:5001/Home/Tags?qq=SkillTree
            //series.labels.template.url = "https://stackoverflow.com/questions/tagged/{word}";

            series.labels.template.url = surl + "?qq={word}";
            series.labels.template.urlTarget = "_blank";
            series.labels.template.tooltipText = "{word}: {value}";

            var hoverState = series.labels.template.states.create("hover");
            hoverState.properties.fill = am4core.color("#FF0000");

            //var subtitle = chart.titles.create();
            //subtitle.text = "(click to open)";

            //var title = chart.titles.create();
            //title.text = "Tag Cloud";
            //title.fontSize = 20;
            //title.fontWeight = "800";
        });
    }

    tagCloidInit();
});