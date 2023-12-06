function out = convolution_combine(input, realW, imaginaryW, weights)

    % Construct kernel from real and imagenary precomputed weights
    kernel = realW + 1j * imaginaryW;
    
    % Horizontal Blur 𝐹(𝑥)=𝑒^(−𝑎𝑥^2 ) (cos⁡(𝑏𝑥^2 )+𝑖 sin⁡(𝑏𝑥^2 ) )
    tmp_out = convolution(input, kernel, 'same');

    % Vertical Blur 𝐹(𝑥)=𝑒^(−𝑎𝑥^2 ) (cos⁡(𝑏𝑥^2 )+𝑖 sin⁡(𝑏𝑥^2 ) )
    tmp_out = convolution(tmp_out, transpose(kernel), 'same');
    
    % Combine the with weights 𝐶𝑜𝑙𝑜𝑟(𝑥)=𝐴〖∗𝐹〗_𝑟𝑒𝑎𝑙 (𝑥)+𝐵〖∗𝐹〗_𝑖𝑚𝑎𝑔𝑖𝑛𝑎𝑟𝑦 (𝑥)

    out = weights(1) * real(tmp_out) + weights(2) * imag(tmp_out);