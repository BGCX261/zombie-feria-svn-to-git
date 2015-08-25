<?php
	// variables globales
	$INPUT_DIR		= "input/";
	$OUTPUT_DIR		= "output/";
	$FILE_FACES		= $INPUT_DIR . "faces.csv";
	$TILES_DIR		= $INPUT_DIR . "tiles/";
	$CSV_DELIMITER	= ";";
	$IMG_SPACE		= "espace.png";
	$TILE_HEIGHT_M	= 12;		// en mètres
	$TILE_HEIGHT	= 777;		// en pixels
	$TILE_WIDTH		= 203;		// en pixels
	$PNG_QUALITY	= 9;		// 0 = aucune compression => 9

	// on récupère la liste des images contenues dans le répertoire des tiles
	$tiles = find_all_files($TILES_DIR);
	$tilesImg = array();
	foreach($tiles as $index => $filename)
	{
		if ($filename == $IMG_SPACE)
			unset($tiles[$index]);
		else
		{
			$tilesImg[] = imagecreatefromstring(file_get_contents($TILES_DIR . $filename));
		}
	}
	
	// on charge l'image contenant les espaces
	$spaceImg = imagecreatefromstring(file_get_contents($TILES_DIR . $IMG_SPACE));
	
	// on récupère le contenu du fichier CSV
	$fileContent = file_get_contents($FILE_FACES);
	
	// on extrait le contenu CSV dans un tableau
	$tabFaces = csv_to_array($fileContent, $CSV_DELIMITER);
	
	// génération des images
	// on va calculer les dimensions des images pour chaque face
	$scale = $TILE_HEIGHT / $TILE_HEIGHT_M;
	foreach($tabFaces as $index => $faceInfos)
	{
		// HEIGHT
		$faceInfos['img_height'] = $TILE_HEIGHT;
		
		// WIDTH : conversion en fonction des mètres
		$faceInfos['img_width'] = round($faceInfos['length'] * $scale);
		
		// FILE NAME
		$faceInfos['filename'] = $faceInfos['code'] . "_" . $faceInfos['face'] . ".png";
		
		// on calcule le nombre de tiles qu'il faut mettre dans l'image
		$nbTiles = floor($faceInfos['img_width'] / $TILE_WIDTH);
		
		// on calcule le nombre d'espaces
		if ($nbTiles > 0)
			$nbSpaces = $nbTiles - 1;
		else
			$nbSpaces = 1;
		
		// on calcule la largeur des espaces
		$spaceWidth = floor(($faceInfos['img_width'] - $nbTiles*$TILE_WIDTH) / $nbSpaces);
		
		// on calcule l'espace restant
		$spaceRemaining = $faceInfos['img_width'] - $nbTiles * $TILE_WIDTH - $nbSpaces * $spaceWidth;
		$spaceLeft = floor($spaceRemaining / 2);
		$spaceRight = $spaceRemaining - $spaceLeft;
		
		// aller, c'est parti, on va coller sur notre image les tiles !
		// création de l'image vide
		$imgOut = imagecreatetruecolor($faceInfos['img_width'], $faceInfos['img_height']);
		$currentX = 0;
		
		// s'il n'y a pas de tiles, on met juste une bande vide
		if ($nbTiles == 0)
		{
			for($i=0; $i<$spaceWidth; $i++)
			{
				imagecopymerge($imgOut, $spaceImg, $currentX, 0, 0, 0, 1, $TILE_HEIGHT, 100);
				$currentX++;
			}
		}
		else
		{
			// on met la bande d'espace GAUCHE
			for($i=0; $i<$spaceLeft; $i++)
			{
				imagecopymerge($imgOut, $spaceImg, $currentX, 0, 0, 0, 1, $TILE_HEIGHT, 100);
				$currentX++;
			}
			
			// pour chaque tile
			for($j=0; $j<$nbTiles; $j++)
			{
				// si on a fait la première, on met un espace au début
				if ($j > 0)
				{
					for($i=0; $i<$spaceWidth; $i++)
					{
						imagecopymerge($imgOut, $spaceImg, $currentX, 0, 0, 0, 1, $TILE_HEIGHT, 100);
						$currentX++;
					}
				}
				
				// on prend un index de tile aléatoire
				$indexTile = rand(0, sizeof($tilesImg)-1);
				imagecopymerge($imgOut, $tilesImg[$indexTile], $currentX, 0, 0, 0, $TILE_WIDTH, $TILE_HEIGHT, 100);
				$currentX += $TILE_WIDTH;
			}
			
			// on met la bande d'espace DROITE
			for($i=0; $i<$spaceRight; $i++)
			{
				imagecopymerge($imgOut, $spaceImg, $currentX, 0, 0, 0, 1, $TILE_HEIGHT, 100);
				$currentX++;
			}			
		}
		echo "Width = " . $faceInfos['img_width'] . " VS currentX = $currentX (space left = $spaceLeft, space right = $spaceRight)<br/>";
		// sauvegarde de l'image
		imagepng($imgOut, $OUTPUT_DIR . $faceInfos['filename'], $PNG_QUALITY);
		imagedestroy($imgOut);
		
		$tabFaces[$index] = $faceInfos;
	}
	
	function	csv_to_array($_input, $_delimiter = ';') 
	{ 
		$header = null; 
		$data = array(); 
		$csvData = str_getcsv($_input, "\n"); 
		
		foreach($csvData as $csvLine)
		{ 
			if (is_null($header))
				$header = explode($_delimiter, $csvLine); 
			else
			{ 	
				$items = explode($_delimiter, $csvLine); 
				$prepareData = array();
				foreach($items as $index => $value)
					$prepareData[$header[$index]] = $items[$index]; 
				
				$data[] = $prepareData; 
			} 
		} 
		
		return $data; 
	} 
	
	function	find_all_files($_dir)
	{
		$root = scandir($_dir);
		foreach($root as $value)
		{
			if ($value === '.' || $value === '..')
			{
				continue;
			}
			if (is_file("$_dir/$value"))
			{
				$result[] = $value;
				continue;
			}
		}
		return $result;
	} 
	
?>



<html>
	<head>
		<title>Zombie Feria - Generate Textures</title>
	</head>

	<body>
		<center>
			<h1>Zombie Feria - Texture Generation</h1>
			
			<?php
				echo "TILES<br/>";
				foreach($tiles as $index => $tileFileName)
				{
					echo "$tileFileName<br/>";
				}
			
				echo "<br/>";
				echo "FACES<br/>";
				foreach($tabFaces as $index => $faceBuf)
				{
					echo "Face $index:<br/>";
					foreach($faceBuf as $indexFace => $value)
						echo "- $indexFace => $value<br/>";
				}
			
			?>
			
			<a href="index.php">Back to index</a>
		</center>
	</body>
</html>