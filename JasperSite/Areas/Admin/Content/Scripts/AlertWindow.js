/**
 * 
 *  AlertWindow.js shows bootstrap modal window with custom header, context, and button event. * 
 * 
 */

    // Creates modal pop-up with custom content
   //mainButtonLCass: btn-jasper-delete
// mainButtonIconStyle: fa fa-trash fa-1x
    function getHtmlWindow(header,body,mainButtonClass,mainButtonIconStyle,mainButtonText,closeButtonText) {
     var window = $(
'<div id="alertWindow" class="modal fade" role="dialog" style="z-index:99999!important">'+
   ' <div class="modal-dialog">'+       
       ' <div class="modal-content">'+
            '<div class="modal-header">'+
                '<button type="button" class="close" data-dismiss="modal">&times;</button>'+
                '<h4 class="modal-title" style="margin-left:20px">' + header +'</h4>'+
            '</div>'+
            '<div class="modal-body">'+
              body +
            '</div>'+
            '<div class="modal-footer">'+
                 ' <a id="deleteModalButton" title="'+ mainButtonText +'" class="btn btn-jasper '+ mainButtonClass +'"> <i class="'+ mainButtonIconStyle+'" aria-hidden="true"></i>'+' '+ mainButtonText +'</a>'+
                '<button type="button" class="btn btn-jasper" data-dismiss="modal" title="'+ closeButtonText+ '">'+ closeButtonText +'</button>'+
            '</div>'+
        '</div>'+

    '</div>'+
'</div>'

)

        return window;
    }

    // inserts modal into targetDiv and shows pop-up 
function appendWindow(header, body, action, targetDiv, mainButtonClass, mainButtonIconStyle, mainButtonText, closeButtonText) {
    var window = getHtmlWindow(header, body, mainButtonClass, mainButtonIconStyle, mainButtonText, closeButtonText);
        window.css({
            "display": "block",
        });

        $(targetDiv).html(window);
      
        $("#deleteModalButton").click(action);

        toggleWindow();
       
    }

    // closes or opens modal pop-up
    function toggleWindow() {
        $("#alertWindow").modal("toggle");
    }
         
        
   