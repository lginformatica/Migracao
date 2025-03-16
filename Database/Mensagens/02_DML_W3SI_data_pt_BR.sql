BEGIN TRAN

IF EXISTS (SELECT 1 FROM W3SI.IDIOMA WHERE COD_IDIOMA = 1)
BEGIN
	INSERT INTO W3IN.MENSAGEM_TMP (COD_IDIOMA, COD_MODULO, NOM_PAGINA, NOM_CONTROLE, NOM_PROPRIEDADE, NOM_CHAVE, DSC_MENSAGEM, IND_REVISADA) VALUES (1, 30, NULL, NULL, NULL, 'tituloTreinamento', 'Treinamento', 'N')
	INSERT INTO W3IN.MENSAGEM_TMP (COD_IDIOMA, COD_MODULO, NOM_PAGINA, NOM_CONTROLE, NOM_PROPRIEDADE, NOM_CHAVE, DSC_MENSAGEM, IND_REVISADA) VALUES (1, 30, NULL, NULL, NULL, 'subtituloTreinamento', 'Reinvente sua gest�o de treinamentos utilizando experi�ncias digitais com acesso a indicadores.', 'N')
	INSERT INTO W3IN.MENSAGEM_TMP (COD_IDIOMA, COD_MODULO, NOM_PAGINA, NOM_CONTROLE, NOM_PROPRIEDADE, NOM_CHAVE, DSC_MENSAGEM, IND_REVISADA) VALUES (1, 30, NULL, NULL, NULL, 'mensagemPaginaTreinamento', 'Capacitar seu time de forma atrativa e garantir engajamento tem sido um grande desafio? Mude o processo de aprendizagem dos colaboradores com metodologias de gamifica��o. <br><br>Uma solu��o f�cil de usar que permite centralizar conte�dos, gerenciar treinamentos e ainda obter indicadores para tomada de decis�o. <br><br>Enquanto possibilita uma maior autonomia e visibilidade para os gestores sobre o desenvolvimento de suas equipes, resulta em economia de tempo e recursos financeiros pela disponibiliza��o e gera��o de conhecimento on-line.', 'N')
	INSERT INTO W3IN.MENSAGEM_TMP (COD_IDIOMA, COD_MODULO, NOM_PAGINA, NOM_CONTROLE, NOM_PROPRIEDADE, NOM_CHAVE, DSC_MENSAGEM, IND_REVISADA) VALUES (1, 30, NULL, NULL, NULL, 'acessarSolucao', 'Acessar a solu��o', 'N')
	INSERT INTO W3IN.MENSAGEM_TMP (COD_IDIOMA, COD_MODULO, NOM_PAGINA, NOM_CONTROLE, NOM_PROPRIEDADE, NOM_CHAVE, DSC_MENSAGEM, IND_REVISADA) VALUES (1, 30, NULL, NULL, NULL, 'tituloClave', 'Perfil Comportamental', 'N')
	INSERT INTO W3IN.MENSAGEM_TMP (COD_IDIOMA, COD_MODULO, NOM_PAGINA, NOM_CONTROLE, NOM_PROPRIEDADE, NOM_CHAVE, DSC_MENSAGEM, IND_REVISADA) VALUES (1, 30, NULL, NULL, NULL, 'subtituloClave', 'Tenha acesso ao mapeamento de perfil dos seus colaboradores e ganhe agilidade e qualidade na tomada de decis�o.', 'N')
	INSERT INTO W3IN.MENSAGEM_TMP (COD_IDIOMA, COD_MODULO, NOM_PAGINA, NOM_CONTROLE, NOM_PROPRIEDADE, NOM_CHAVE, DSC_MENSAGEM, IND_REVISADA) VALUES (1, 30, NULL, NULL, NULL, 'mensagemPaginaClave', 'Que tal acelerar o alcance de resultados da sua empresa entendendo o potencial do seu time e onde cada um atua com melhor desempenho? <br><br>Com a solu��o, � poss�vel conhecer profundamente o perfil comportamental e as principais caracter�sticas dos seus colaboradores, j� que oferece dados importantes para identificar talentos e gaps a serem tratados. <br><br>Alguns benef�cios s�o: entender as aspira��es de carreira e prop�sito de vida dos funcion�rios, mapear perfis alinhados � cultura, estrat�gias e necessidades da empresa e tomar decis�es mais acertadas sobre contrata��o,  desenvolvimento e sucess�o.', 'N')
	
	-- MENSAGENS - PORTUGU�S (MANTENHA ESTE COMENT�RIO COMO A �LTIMA LINHA)
END
COMMIT
