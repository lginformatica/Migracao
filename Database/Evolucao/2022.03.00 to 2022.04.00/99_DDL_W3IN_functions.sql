-- VERIFICA��O DA VERS�O ATUAL DO BANCO DE DADOS
SET NOEXEC OFF

IF W3IN.FN_VERSAO_IN() <> '2022.3.0' BEGIN
	PRINT 'Vers�o atual: ' + W3IN.FN_VERSAO_IN()
	RAISERROR('ERRO: Estas altera��es s�o incompat�veis com a vers�o atual.', 18, 1) WITH NOWAIT
	SET NOEXEC ON
END
GO


IF EXISTS(SELECT OBJECT_ID FROM SYS.OBJECTS WHERE NAME = 'FN_VERSAO_IN' AND TYPE = 'FN')
	DROP FUNCTION  W3IN.FN_VERSAO_IN;
GO

CREATE FUNCTION W3IN.FN_VERSAO_IN()
RETURNS VARCHAR(20)
BEGIN
	RETURN '2022.4.0'
END