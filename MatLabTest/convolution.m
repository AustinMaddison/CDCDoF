function out = convolution(input, kernel, cfg)
    kernel = gpuArray(kernel);
    redChannel = gpuArray(input(:, :, 1));
    greenChannel = gpuArray(input(:, :, 2));
    blueChannel = gpuArray(input(:, :, 3));
    
    r = conv2(double(redChannel), kernel, cfg);
    g = conv2(double(greenChannel), kernel, cfg);
    b = conv2(double(blueChannel), kernel, cfg);
    
    out(:,:,1) = r;
    out(:,:,2) = g;
    out(:,:,3) = b;
