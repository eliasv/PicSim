;**********************
;Flash PORTC .5 seconds
;**********************
list p=16F877A
include "p16F877A.inc"
__config _CP_OFF&_WDT_OFF&_PWRTE_ON&_HS_OSC ;HS for 8MHz crystal
;temp variables
cblock 0x20
	counter  
endc
org 0 ;start a 0
	bsf STATUS, RP0 ;switch to Bank1
	bcf STATUS, RP1
	clrf TRISC		;set PORTC to output
	bcf STATUS, RP0 ;switch to Bank0
	clrf PORTC		;PORTC = 0
start	
	call delay250ms	;call .25s delay
	comf PORTC		;invert PORTC
	call delay250ms	;switch to Bank1
	goto start		;infinite loop

;**************	subroutines **************

;250ms subroutine, call 1ms routine 250 times
delay250ms
	movlw d'250'
	movwf counter
again1ms
	call delay1ms
	decfsz counter,f
	goto again1ms
	return

;1ms subroutine
delay1ms
	movlw 0xf9
dec1
	addlw 0xff	;subtracts 1 from W register
	btfss STATUS, Z
	goto dec1
	return	
end