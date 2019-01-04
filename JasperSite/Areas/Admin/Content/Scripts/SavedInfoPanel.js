
        /*
         This method shows (appends to header) slideUp notification box with specified text and style (ie. alert-warning).
         When the notification box is not visible, it is automatically removed from DOM.
         */
module.exports= function savedInfo(text, style) {         

            var panel = $('<div name="savedInfoPanel" class="j-flex-row j-secondary-header" >'+
                                '<div class="alert '+style+'" role="alert" style="margin:0px;width:100%">'+
                                text +
                                '</div>'+        
                           '</div>' );

            panel.css({
                "display": "none",                
            });

            $("#headerPanel").append(panel);

            // Animation and removal from DOM
         panel.slideDown("slow").delay(2500).slideUp("slow", function () {
                 $("#headerPanel").children("[name='savedInfoPanel']:first").remove();
            }); 

            // After submit, some submit buttons have still focus efect
            $(":submit").blur();
        }
