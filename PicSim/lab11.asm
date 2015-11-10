;**********************
;Turn on all PORTC
;**********************
list p=16F877A
include "p16F877A.inc"
__config _CP_OFF&_WDT_OFF&_PWRTE_ON&_HS_OSC ;HS for 8MHz crystal

org 0 ;start a 0
	bsf STATUS, RP0 ;switch to Bank1
	bcf STATUS, RP1
	clrf TRISC		;set PORTC to output
	bcf STATUS, RP0 ;switch to Bank0
	movlw 0xFF
	movwf PORTC		;PORTC = 0xFF
start	
	
	goto start		;infinite loop


end