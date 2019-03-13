#!/usr/bin/python

import glob, re, json, os
import PIL.Image

pngs_paths = glob.glob("../Sprites/raw/*.png")
with open("image-descs.json") as f:
	image_descs = json.load(f)

offsets = [
	# First fill in corners.
	(-1, -1), (-1, +1),
	(+1, -1), (+1, +1),
	# Then fill in the edges.
	(+1, 0), (-1, 0),
	(0, +1), (0, -1),
	# Finally fill in the main image in the center.
	(0, 0),
]

def make_processed_version(tile_size, source_image):
	# Assert proper sizing.
	assert source_image.size[0] % tile_size[0] == 0
	assert source_image.size[1] % tile_size[1] == 0
	tiles = source_image.size[0] / tile_size[0], source_image.size[1] / tile_size[1]
	print "Processing: %s (mode=%r, %ix%i in %ix%i tiles of %ix%i)" % (
		png_path,
		source_image.mode,
		source_image.size[0], source_image.size[1],
		tiles[0], tiles[1],
		tile_size[0], tile_size[1],
	)

	output_size = source_image.size[0] + 2*tiles[0], source_image.size[1] + 2*tiles[1]
	new_image = PIL.Image.new(source_image.mode, output_size)

	for y in xrange(tiles[1]):
		for x in xrange(tiles[0]):
			for sub_x, sub_y in offsets:
				source_box = (
					x       * tile_size[0], y       * tile_size[1],
					(x + 1) * tile_size[0], (y + 1) * tile_size[1],
				)
				part = source_image.crop(source_box)
				dest_offset = (
					1 + (tile_size[0] + 2) * x + sub_x,
					1 + (tile_size[1] + 2) * y + sub_y,
				)
				new_image.paste(part, dest_offset)
	return new_image

for png_path in pngs_paths:
	dest_path = png_path.replace("raw/", "")
	assert dest_path != png_path
	if os.path.getmtime(dest_path) > os.path.getmtime(png_path):
		print "Not newer. Skipping:", png_path
		continue

	source_image = PIL.Image.open(png_path)
	base_name, = re.search("([^/]+[.]png)", png_path).groups()
	tile_size = image_descs[base_name]
	if tile_size is False:
		print "Copying image:", png_path
		new_image = source_image
	else:
		new_image = make_processed_version(tile_size, source_image)

	print "Writing %s -> %s" % (png_path, dest_path)
#	assert not os.path.exists(dest_path)
	new_image.save(dest_path)

