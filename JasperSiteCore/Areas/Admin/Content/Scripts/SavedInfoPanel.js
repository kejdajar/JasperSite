
        function savedInfo(text,style) {

         

            var panel = $(`


            <div class="j-flex-row j-secondary-header" >

                        <div class="alert `+ style +`" role="alert" style="margin:0px;width:100%">
                       `+ text +`
                        </div>
                
            </div>


`);

            panel.css({
                "display": "none",                
            });

            $("#headerPanel").append(panel);
            panel.slideDown("slow").delay(2500).slideUp("slow");

            $(":submit").blur();/* After submit the submit button had still focus efect*/
        }
