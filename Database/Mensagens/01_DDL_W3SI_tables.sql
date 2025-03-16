--APAGA A TABELA W3IN.MENSAGEM_TMP CASO ELA JÁ EXISTA NO BANCO
IF EXISTS (SELECT 1
			FROM SYSOBJECTS
			WHERE  ID = OBJECT_ID('W3IN.MENSAGEM_TMP')
			AND TYPE IN ('U'))
	DROP TABLE W3IN.MENSAGEM_TMP
GO

--CRIA A TABELA TEMPORÁRIA PARA INSERÇÃO DAS MENSAGENS
CREATE TABLE W3IN.MENSAGEM_TMP (
       COD_MENSAGEM         int IDENTITY,
       COD_IDIOMA           int NOT NULL,
       COD_MODULO           int NOT NULL,
       NOM_PAGINA           varchar(200) NULL,
       NOM_CONTROLE         varchar(100) NULL,
       NOM_PROPRIEDADE      varchar(100) NULL,
       NOM_CHAVE            varchar(255) NOT NULL,
       DSC_MENSAGEM         varchar(2000) NOT NULL,
       IND_REVISADA CHAR(1) NOT NULL
                                   CONSTRAINT DF_MENSAGEM_IND_REVISADA_TMP 
                                          DEFAULT 'N'
                                   CONSTRAINT CK_MENSAGEM_IND_REVISADA_TMP 
                                          CHECK (IND_REVISADA IN ('S', 'N')),
       CONSTRAINT PK_MENSAGEM_TMP 
              PRIMARY KEY (COD_MENSAGEM)
)
go
