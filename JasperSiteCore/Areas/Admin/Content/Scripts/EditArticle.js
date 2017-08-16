
$.datepicker.regional['cs'] = {
    closeText: 'Cerrar',
    prevText: 'Předchozí',
    nextText: 'Další',
    currentText: 'Hoy',
    monthNames: ['Leden', 'Únor', 'Březen', 'Duben', 'Květen', 'Červen', 'Červenec', 'Srpen', 'Září', 'Říjen', 'Listopad', 'Prosinec'],
    monthNamesShort: ['Le', 'Ún', 'Bř', 'Du', 'Kv', 'Čn', 'Čc', 'Sr', 'Zá', 'Ří', 'Li', 'Pr'],
    dayNames: ['Neděle', 'Pondělí', 'Úterý', 'Středa', 'Čtvrtek', 'Pátek', 'Sobota'],
    dayNamesShort: ['Ne', 'Po', 'Út', 'St', 'Čt', 'Pá', 'So',],
    dayNamesMin: ['Ne', 'Po', 'Út', 'St', 'Čt', 'Pá', 'So'],
    weekHeader: 'Sm',
    dateFormat: 'dd.mm.yy',
    firstDay: 1,
    isRTL: false,
    showMonthAfterYear: false,
    yearSuffix: ''
};

$.datepicker.regional['sk'] = {
    closeText: 'Zavrieť',
    prevText: '<Predchádzajúci',
    nextText: 'Nasledujúci>',
    currentText: 'Dnes',
    monthNames: ['Január', 'Február', 'Marec', 'Apríl', 'Máj', 'Jún',
        'Júl', 'August', 'September', 'Október', 'November', 'December'],
    monthNamesShort: ['Jan', 'Feb', 'Mar', 'Apr', 'Máj', 'Jún',
        'Júl', 'Aug', 'Sep', 'Okt', 'Nov', 'Dec'],
    dayNames: ['Nedel\'a', 'Pondelok', 'Utorok', 'Streda', 'Štvrtok', 'Piatok', 'Sobota'],
    dayNamesShort: ['Ned', 'Pon', 'Uto', 'Str', 'Štv', 'Pia', 'Sob'],
    dayNamesMin: ['Ne', 'Po', 'Ut', 'St', 'Št', 'Pia', 'So'],
    weekHeader: 'Ty',
    dateFormat: 'dd.mm.yy',
    firstDay: 0,
    isRTL: false,
    showMonthAfterYear: false,
    yearSuffix: ''
};
/* Enables datepicker on date field */
$.datepicker.setDefaults($.datepicker.regional['cs']);
$(document).ready(function () {


    $(".tbPublishDate").datepicker();



});

    function savedInfo()
    {

      

        //var dialog = $("<div> uloženo </div>");
        //dialog.css({
        //    "position": "absolute",           
        //    "background-color": "red",
        //    "width": "100px",
        //    "height": "100px",
        //    "display":"none"
        //});

        //var x = $(window).width()/2   - (dialog.outerWidth() / 2);
        //var y = $(window).height()/2 - (dialog.outerHeight() / 2);
        // dialog.css({            
        //    "left": x,
        //    "top": y,            
        //});

        // dialog.appendTo('body');
        // dialog.slideDown("slow");



        var panel = $(`<div class="j-flex-row j-secondary-header">

             <div class="j-header-group">

                 <div class="j-header-group-item">
                 <span style='color:white'>  <b>Uloženo</b> </span>
                 </div>                 

             </div>
         </div>`);

        panel.css({
            "display": "none",
            "background-color": "#8BC34A",     
           
            "font-size":"1.3em"
        });

        $("#headerPanel").append(panel);
        panel.slideDown("slow").delay(2000).slideUp("slow");

    }